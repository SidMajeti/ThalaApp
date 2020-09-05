#import <AVFoundation/AVFoundation.h>
#import <AudioToolbox/AudioToolbox.h>

@interface SoundPlayer:NSObject
{
    AVAudioPlayerNode *audioPlayerNode;
    AVAudioFile *audioFileMainClick;
    AVAudioEngine *audioEngine;
}
@end


@implementation SoundPlayer

static SoundPlayer *_sharedInstance;
double firstime = 0;
float currSpeed = 0.0f;
int loopcount = 0;

+(SoundPlayer*) sharedInstance
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _sharedInstance = [[SoundPlayer alloc] init];
    });
    return _sharedInstance;
}

-(id) init
{
    self = [super init];
    return self;
}


-(void)initSoundPlayer:(NSString*) soundFilePath
{
    [[AVAudioSession sharedInstance]
    setCategory: AVAudioSessionCategoryPlayback
          error: nil];
    
    NSURL *soundFileURL = [NSURL fileURLWithPath:soundFilePath];
    audioFileMainClick = [[AVAudioFile alloc] initForReading:soundFileURL error:nil];
            
    
    audioPlayerNode = [[AVAudioPlayerNode alloc] init];
    
    audioEngine = [[AVAudioEngine alloc] init];
    [audioEngine attachNode:audioPlayerNode];
    
    [audioEngine connect:audioPlayerNode to:[audioEngine mainMixerNode] format:[audioFileMainClick processingFormat]];
    
    [audioEngine prepare];
    
    [audioEngine startAndReturnError:nil];
}

-(AVAudioPCMBuffer*)generateBuffer:(float) bpm{
    NSError *error = nil;
    [audioFileMainClick setFramePosition:0];
    
    int beatLength = AVAudioFrameCount(audioFileMainClick.processingFormat.sampleRate * 60 / bpm);
    AVAudioPCMBuffer *buffermainclick = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:beatLength];
    
    [audioFileMainClick readIntoBuffer:buffermainclick error:&error];
    [buffermainclick setFrameLength:beatLength];
    
    return buffermainclick;
}


//very slight lag
//prevent user from going over a certain speed: 200?
-(void)playSound:(float) speed :(NSString*) tag :(int)khandaCount :(int)misraCount{
    loopcount = 0;
    currSpeed = speed;
    if(speed > 90.0f){
        speed -= 4.0* (speed - 100)/ 100.0;
    }
    if(![tag isEqualToString:@"KhandaTag"]){
        speed -= 5.50f * speed/75.0;
    }
    else{
        speed -= 1.00f * speed/75.0;
    }
    dispatch_time_t delay;
    if(misraCount == 3){
        NSLog(@"Tag worked");
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.15*(75.0/speed)*NSEC_PER_SEC);
    }
    else if(misraCount == 1){
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.08*(75.0/speed)*NSEC_PER_SEC);
    }
    else if(misraCount == 2){
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.04*(75.0/speed)*NSEC_PER_SEC);
    }
    else if([tag isEqualToString:@"MisraTag"]){
        delay = dispatch_time(DISPATCH_TIME_NOW, 0*NSEC_PER_SEC);
    }
    else if(khandaCount == 2){
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.2*(75.0/speed)*NSEC_PER_SEC);
    }
    else if(khandaCount == 3){
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.10*(75.0/speed)*NSEC_PER_SEC);
    }
    else if([tag isEqualToString:@"KhandaTag"]){
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.15*(75.0/speed)*NSEC_PER_SEC);
    }
    else if (speed > 66.0f){
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.3*pow((75.0/speed), 2)*NSEC_PER_SEC);
    }
    else{
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.5*(75.0/speed)*NSEC_PER_SEC);
    }
    dispatch_after(delay, dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        AVAudioPCMBuffer *buffer;
//        if(khandaCount == 2 || misraCount == 1){
//            buffer = [_sharedInstance generateBuffer:speed*2];
//        }
        buffer = [_sharedInstance generateBuffer:speed];
        if(audioEngine.isRunning){
            if ([audioPlayerNode isPlaying]) {
                [audioPlayerNode scheduleBuffer:buffer atTime:nil options:AVAudioPlayerNodeBufferInterrupts completionHandler:nil];
            } else {
                [audioPlayerNode play];
            }
            if(!([tag isEqualToString:@"KhandaTag"] || [tag isEqualToString:@"MisraTag"])){
                [audioPlayerNode scheduleBuffer:buffer atTime:nil options:AVAudioPlayerNodeBufferLoops completionHandler:nil];
            }
            else{
                [audioPlayerNode scheduleBuffer:buffer completionHandler:nil];
            }
        }
    });
    
}

-(bool)isPlaying{
    return [audioPlayerNode isPlaying];
}
-(void)stopSound{
    if([audioEngine isRunning]){
        [audioPlayerNode stop];
    }
}

-(float)getSpeed{
    return currSpeed;
}

-(void) unmuteSound{
    [audioEngine startAndReturnError:nil];
}

-(void) muteSound{
    [audioEngine pause];
}

-(void) setLoopCount{
    loopcount += 1;
}

-(int) getLoopCount{
    return loopcount;
}


@end

extern "C"
{
    void InitSPlayer(char* soundFilePath){
        [[SoundPlayer sharedInstance] initSoundPlayer:[NSString stringWithUTF8String:soundFilePath]];
    }
//    void IOSChangeSpeed(float speed){
//        [[SoundPlayer sharedInstance] changeSpeed:speed];
//    }
    void IOSMuteSound(){
        [[SoundPlayer sharedInstance] muteSound];
    }
    void IOSUnMuteSound(){
        [[SoundPlayer sharedInstance] unmuteSound];
    }
    void IOSStopSound(){
        [[SoundPlayer sharedInstance] stopSound];
    }
    void IOSPlaySound(float speed, char* tag, int khandaCount, int misraCount){
        [[SoundPlayer sharedInstance] playSound:speed :[NSString stringWithUTF8String:tag]:khandaCount:misraCount];
    }
    bool IOSIsPlaying(){
        return [[SoundPlayer sharedInstance] isPlaying];
    }
    float IOSGetSpeed(){
        return [[SoundPlayer sharedInstance] getSpeed];
    }
    int IOSGetLoopCount(){
        return [[SoundPlayer sharedInstance] getLoopCount];
    }
    void IOSSetLoopCount(){
        [[SoundPlayer sharedInstance] setLoopCount];
    }
}

