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
    
    int beatLength = AVAudioFrameCount(48000 * 60.0 / bpm);
    AVAudioPCMBuffer *buffermainclick = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:beatLength];
    
    [audioFileMainClick readIntoBuffer:buffermainclick error:&error];
    [buffermainclick setFrameLength:beatLength];
    
    return buffermainclick;
}

-(int) mod:(int)a :(int) b{
    int ret = a % b;
    if(ret < 0)
        ret+=b;
    return ret;
}
-(AVAudioPCMBuffer*)generateKhandaBuffer:(float) bpm dis:(int) displacement{
    NSError *error = nil;
    [audioFileMainClick setFramePosition:0];
    
    int beatLength = 48000 * 60.0 / bpm;
    AVAudioPCMBuffer *regBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:beatLength];

    
    AVAudioPCMBuffer *khandaBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:int(2.5 * 48000 * 60.0 / bpm)];
    khandaBuffer.frameLength = int(2.5 * 48000 * 60.0 / bpm);
    
    [audioFileMainClick readIntoBuffer:regBuffer error:&error];
    [regBuffer setFrameLength:beatLength];
    
    for(int i = 0; i < [[audioFileMainClick processingFormat] channelCount]; i++){
        memcpy(khandaBuffer.floatChannelData[i] + [_sharedInstance mod: -displacement :khandaBuffer.frameLength], regBuffer.floatChannelData[i] + int(1000*pow(bpm/75.0,0.5)), regBuffer.frameLength - int(1000*pow(bpm/75.0,0.5)));
        memcpy(khandaBuffer.floatChannelData[i] + [_sharedInstance mod:(regBuffer.frameLength - displacement) :khandaBuffer.frameLength], regBuffer.floatChannelData[i] + int(2000*pow(bpm/75.0,0.5)), int(regBuffer.frameLength/2.0));
        memcpy(khandaBuffer.floatChannelData[i] +  [_sharedInstance mod:(int(1.5 * 48000 * 60.0 / bpm) - displacement) :khandaBuffer.frameLength], regBuffer.floatChannelData[i] + int(2000*pow(bpm/75.0,0.5)), regBuffer.frameLength - int(2000*pow(bpm/75.0,0.5)));
    }
    return khandaBuffer;
}

-(AVAudioPCMBuffer*)generateMisraBuffer:(float) bpm dis:(int) displacement{
    NSError *error = nil;
    [audioFileMainClick setFramePosition:0];
    
    int beatLength = 48000 * 60.0 / bpm;
    AVAudioPCMBuffer *regBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:beatLength];

    
    AVAudioPCMBuffer *misraBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:int(3.5 * 48000 * 60.0 / bpm)];
    misraBuffer.frameLength = int(3.5 * 48000 * 60.0 / bpm);
    
    [audioFileMainClick readIntoBuffer:regBuffer error:&error];
    [regBuffer setFrameLength:beatLength];
    
    for(int i = 0; i < [[audioFileMainClick processingFormat] channelCount]; i++){
        memcpy(misraBuffer.floatChannelData[i] + [_sharedInstance mod:-displacement :misraBuffer.frameLength], regBuffer.floatChannelData[i] + int(2000*pow(bpm/90.0,2)), int(0.5 * regBuffer.frameLength));
        memcpy(misraBuffer.floatChannelData[i] + [_sharedInstance mod:(int(0.5 * 48000 * 60.0 / bpm) -displacement) :misraBuffer.frameLength], regBuffer.floatChannelData[i], regBuffer.frameLength);
        memcpy(misraBuffer.floatChannelData[i] + [_sharedInstance mod:(int(1.5 * 48000 * 60.0 / bpm) -displacement) :misraBuffer.frameLength] , regBuffer.floatChannelData[i], regBuffer.frameLength);
        memcpy(misraBuffer.floatChannelData[i] +  [_sharedInstance mod:(int(2.5 * 48000 * 60.0 / bpm) - displacement) :misraBuffer.frameLength], regBuffer.floatChannelData[i], regBuffer.frameLength);
    }
    return misraBuffer;
}


-(void)playSound:(float) speed :(NSString*) tag :(int)khandaCount :(int)misraCount{
    loopcount = 0;
    currSpeed = speed;
    dispatch_time_t delay;
    
    if(!([tag isEqualToString:@"KhandaTag"] || [tag isEqualToString:@"MisraTag"])){
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.3*pow((75.0/speed), 1)*NSEC_PER_SEC);
    }
    else{
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.2*pow((75.0/speed), 2)*NSEC_PER_SEC);
    }
    
    dispatch_after(delay, dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        AVAudioPCMBuffer *buffer;
        if(khandaCount == 1){
            NSLog(@"Khanda1");
            buffer = [_sharedInstance generateKhandaBuffer:speed dis:0];
        }
        else if(khandaCount == 2){
            NSLog(@"Khanda2");
            buffer = [_sharedInstance generateKhandaBuffer:speed dis:int(48000 * 60.0 / speed)];
        }
        else if(khandaCount == 3){
            NSLog(@"Khanda3");
            buffer = [_sharedInstance generateKhandaBuffer:speed dis:int(1.5 * 48000 * 60.0 / speed)];
        }
        else if(misraCount == 1){
            NSLog(@"Misra1");
            buffer = [_sharedInstance generateMisraBuffer:speed dis:0];
        }
        else if(misraCount == 2){
            NSLog(@"Misra2");
            buffer = [_sharedInstance generateMisraBuffer:speed dis:int(0.5*48000 * 60.0 / speed)];
        }
        else if(misraCount == 3){
            NSLog(@"Misra3");
            buffer = [_sharedInstance generateMisraBuffer:speed dis:int(1.5 * 48000 * 60.0 / speed)];
        }
        else if(misraCount == 4){
            NSLog(@"Misra4");
            //have to multiply float before due to loss of precision
            buffer = [_sharedInstance generateMisraBuffer:speed dis:int(2.5 * 48000 * 60.0 / speed)];
        }
        else
            buffer = [_sharedInstance generateBuffer:speed];
        if(audioEngine.isRunning){
            if ([audioPlayerNode isPlaying]) {
                [audioPlayerNode scheduleBuffer:buffer atTime:nil options:AVAudioPlayerNodeBufferInterrupts completionHandler:nil];
            } else {
                [audioPlayerNode play];
            }
            [audioPlayerNode scheduleBuffer:buffer atTime:nil options: AVAudioPlayerNodeBufferLoops completionHandler:nil];
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
    [audioEngine attachNode:audioPlayerNode];

    [audioEngine connect:audioPlayerNode to:[audioEngine mainMixerNode] format:[audioFileMainClick processingFormat]];

    [audioEngine prepare];

    [audioEngine startAndReturnError:nil];
}

-(void) muteSound{
    [audioEngine stop];
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

