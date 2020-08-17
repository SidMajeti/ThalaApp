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
-(void)playSound:(float) speed :(NSString*) tag{
    currSpeed = speed;
    if(speed > 100.0f){
        speed -= 2.0* (speed - 100)/ 100.0;
    }
    
    dispatch_time_t delay;
    if([tag isEqualToString:@"MisraTag"]){
        NSLog(@"Tag worked");
        delay = dispatch_time(DISPATCH_TIME_NOW, 0);
    }
    else if (speed > 66.0f){
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.5*pow((72.0/speed), 2)*NSEC_PER_SEC);
    }
    else{
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.5*(72.0/speed)*NSEC_PER_SEC);
    }
    dispatch_after(delay, dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        AVAudioPCMBuffer *buffer = [_sharedInstance generateBuffer:speed];
        if(audioEngine.isRunning){
//            NSLog(@"Audio Engine is running");
            if ([audioPlayerNode isPlaying]) {
//                NSLog(@"Audio is playing");
                [audioPlayerNode stop];
                [audioPlayerNode play];
                [audioPlayerNode scheduleBuffer:buffer atTime:nil options:AVAudioPlayerNodeBufferInterrupts completionHandler:nil];
            } else {
//                NSLog(@"Audio is playing");
//                NSLog(@"Speed is %f", speed);
                [audioPlayerNode play];
                [audioPlayerNode scheduleBuffer:buffer atTime:nil options:AVAudioPlayerNodeBufferLoops completionHandler:nil];
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
//        NSLog(@"AudioPlayer is stopped");
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

//-(bool)isPlaying{
//    return player.isPlaying;
//}

/*-(void)seekTo{
    player.currentTime = 0;
}*/

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
    void IOSPlaySound(float speed, char* tag){
        [[SoundPlayer sharedInstance] playSound:speed :[NSString stringWithUTF8String:tag]];
    }
    bool IOSIsPlaying(){
        return [[SoundPlayer sharedInstance] isPlaying];
    }
    float IOSGetSpeed(){
        return [[SoundPlayer sharedInstance] getSpeed];
    }
//    void SeekTo(){
//        [[SoundPlayer sharedInstance] seekTo];
//    }
}

