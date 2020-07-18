#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <AudioToolbox/AudioToolbox.h>

@interface SoundPlayer : NSObject
{
    AVPlayer *player;
    AVAudioSession *session;
    AVQueuePlayer *queuePlayer;
    NSURL *soundFileURL;
    AVPlayerItem *lastitem;
}
@end


@implementation SoundPlayer

static SoundPlayer *_sharedInstance;

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

-(void)playerDidFinishPlaying:(NSNotification*) notification{
    queuePlayer.actionAtItemEnd = AVPlayerActionAtItemEndNone;
    AVPlayerItem *playerItem = [[AVPlayerItem alloc] initWithURL:soundFileURL];
    playerItem.audioTimePitchAlgorithm = AVAudioTimePitchAlgorithmSpectral;
    [queuePlayer insertItem:playerItem afterItem:lastitem];
    NSLog(@"QueuePlayerItem added");
    lastitem = playerItem;
}

-(void)initSoundPlayer:(NSString*) soundFilePath
{
    soundFileURL = [NSURL fileURLWithPath:soundFilePath];
    session = [AVAudioSession sharedInstance];
    [session setCategory:AVAudioSessionCategoryPlayback error:nil];
    queuePlayer = [[AVQueuePlayer alloc] init];
    queuePlayer.actionAtItemEnd = AVPlayerActionAtItemEndNone;
    lastitem = nil;
    for(int i = 0; i< 100; i++){
        AVPlayerItem *playerItem = [[AVPlayerItem alloc] initWithURL:soundFileURL];
        playerItem.audioTimePitchAlgorithm = AVAudioTimePitchAlgorithmSpectral;
        [queuePlayer insertItem:playerItem afterItem:lastitem];
        lastitem = playerItem;
    }
//    [[NSNotificationCenter defaultCenter] addObserver:self selector: @selector(playerDidFinishPlaying:) name:AVPlayerItemDidPlayToEndTimeNotification object:nil];
}

//some reason plays multiple times on load
-(void)playSound:(float) speed{
    [queuePlayer play];
    [queuePlayer advanceToNextItem];
    dispatch_async(dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0ul), ^{
        AVPlayerItem *playerItem = [[AVPlayerItem alloc] initWithURL:soundFileURL];
        playerItem.audioTimePitchAlgorithm = AVAudioTimePitchAlgorithmSpectral;
        [queuePlayer insertItem:playerItem afterItem:lastitem];
        lastitem = playerItem;
    });
}

-(void)stopSound{
    [queuePlayer pause];
}

-(void)muteSound{
    [queuePlayer setMuted:true];
}

-(void)unMuteSound{
    [queuePlayer setMuted:false];
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
    void IOSStopSound(){
        [[SoundPlayer sharedInstance] stopSound];
    }
    void IOSPlaySound(float speed){
        [[SoundPlayer sharedInstance] playSound:speed];
    }
    void IOSMuteSound(){
        [[SoundPlayer sharedInstance] muteSound];
    }
    void IOSUnMuteSound(){
        [[SoundPlayer sharedInstance] unMuteSound];
    }
//    bool IOSIsPlaying(){
//        return [[SoundPlayer sharedInstance] isPlaying];
//    }
//    void SeekTo(){
//        [[SoundPlayer sharedInstance] seekTo];
//    }
}
