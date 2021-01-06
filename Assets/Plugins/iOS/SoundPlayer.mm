#import <AVFoundation/AVFoundation.h>
#import <AudioToolbox/AudioToolbox.h>

@interface SoundPlayer:NSObject
{
    AVAudioPlayerNode *audioPlayerNode;
    AVAudioFile *audioFileMainClick;
    AVAudioFile *audioFileFirstClick;
    AVAudioEngine *audioEngine;
//    AVAudioSession *audioSession;
    NSURL *soundFileURL;
    NSURL *soundFileURL2;
}
@end


@implementation SoundPlayer

static SoundPlayer *_sharedInstance;
double firstime = 0;
float currSpeed = 0.0f;
int loopcount = 0;
bool isStopped = false;

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


-(void)initSoundPlayer:(NSString*) otherSound :(NSString*) firstSound
{
    [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayback withOptions:AVAudioSessionCategoryOptionDefaultToSpeaker error:nil];
    
    [[AVAudioSession sharedInstance] setCategory:AVAudioSessionCategoryPlayback withOptions:AVAudioSessionCategoryOptionAllowBluetoothA2DP error:nil];
  
    [[AVAudioSession sharedInstance] setActive:YES error:nil];
    
    soundFileURL = [NSURL fileURLWithPath:otherSound];
    audioFileMainClick = [[AVAudioFile alloc] initForReading:soundFileURL error:nil];
    
    soundFileURL2 = [NSURL fileURLWithPath:firstSound];
    audioFileFirstClick = [[AVAudioFile alloc] initForReading:soundFileURL2 error:nil];
    
    audioPlayerNode = [[AVAudioPlayerNode alloc] init];
    
    audioEngine = [[AVAudioEngine alloc] init];
    [audioEngine attachNode:audioPlayerNode];
    
    [audioEngine connect:audioPlayerNode to:[audioEngine mainMixerNode] format:[audioFileMainClick processingFormat]];
    
    [audioEngine prepare];
    
    [audioEngine startAndReturnError:nil];
    
    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(handleInterruption:) name:AVAudioSessionInterruptionNotification object:nil];
}

-(void) handleInterruption: (NSNotification*) notification{
    NSDictionary *interuptionDict = notification.userInfo;
    NSInteger interuptionType = [[interuptionDict valueForKey:AVAudioSessionInterruptionTypeKey] integerValue];
    switch (interuptionType) {
        case AVAudioSessionInterruptionTypeBegan:
//            NSLog(@"Audio Session Interruption case started.");
            [audioEngine pause];
            [audioPlayerNode stop];
            break;

        case AVAudioSessionInterruptionTypeEnded:
//            NSLog(@"Audio Session Interruption case ended.");
            [audioEngine startAndReturnError:nil];
            break;
    }
}

-(AVAudioPCMBuffer*)generateBuffer:(float) bpm :(int[]) thala :(int) sizeOfArr :(int) displacement :(int) laghuCount{
    NSError *error = nil;
    [audioFileMainClick setFramePosition:0];
    [audioFileFirstClick setFramePosition:0];
    
    int beatLength = AVAudioFrameCount(48000 * 60.0 / bpm);
    int totalLength = 0;
    for(int i = 0; i < sizeOfArr; i++){
        if(thala[i] == 1){
            totalLength += laghuCount;
        }
        else if(thala[i] == 2){
            totalLength += 2;
        }
        else{
            totalLength += 1;
        }
    }
    AVAudioPCMBuffer *buffermainclick = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:int(totalLength * 48000 * 60.0 / bpm)];
    
    [buffermainclick setFrameLength:int(totalLength * 48000 * 60.0 / bpm)];
    AVAudioPCMBuffer *firstBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileFirstClick.processingFormat frameCapacity:beatLength];
    
    AVAudioPCMBuffer *regBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:beatLength];
    
    [audioFileMainClick readIntoBuffer:regBuffer error:&error];
    [regBuffer setFrameLength:beatLength];
    [audioFileFirstClick readIntoBuffer:firstBuffer error:&error];
    [firstBuffer setFrameLength:beatLength];
    
    int counter = 0;
    for(int k = 0; k < [[audioFileMainClick processingFormat] channelCount]; k++){
        for (int i = 0; i < sizeOfArr; i++){
            //laghu
            if(thala[i] == 1){
                int j = 0;
                while(j != laghuCount){
                    if(j == 0){
                        memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], firstBuffer.floatChannelData[k], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
                    }
                    else{
                        memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], regBuffer.floatChannelData[k], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
                    }
                    counter += 1;
                    j+= 1;
                }
            }
            //anudrutham
            else if(thala[i] == 0){
                memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], regBuffer.floatChannelData[k], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
                counter += 1;
            }
            //drutham
            else if(thala[i] == 2){
                memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], firstBuffer.floatChannelData[k], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
                counter += 1;
                memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], regBuffer.floatChannelData[k], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
                counter += 1;
            }
        }
    }
    
    return buffermainclick;
}

-(AVAudioPCMBuffer*)generateDoubleBuffer:(float) bpm :(int[]) thala :(int) sizeOfArr :(int) displacement :(int) laghuCount{
    NSError *error = nil;
    [audioFileMainClick setFramePosition:0];
    [audioFileFirstClick setFramePosition:0];
    
    int beatLength = AVAudioFrameCount(48000 * 60.0 / bpm);
    int totalLength = 0;
    for(int i = 0; i < sizeOfArr; i++){
        if(thala[i] == 1){
            totalLength += laghuCount*2;
        }
        else if(thala[i] == 2){
            totalLength += 2*2;
        }
        else{
            totalLength += 1*2;
        }
    }
    AVAudioPCMBuffer *buffermainclick = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:int(totalLength * 48000 * 60.0 / bpm)];
    
    [buffermainclick setFrameLength:int(totalLength * 48000 * 60.0 / bpm)];
    AVAudioPCMBuffer *firstBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileFirstClick.processingFormat frameCapacity:beatLength];
    
    AVAudioPCMBuffer *regBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:beatLength];
    
    [audioFileMainClick readIntoBuffer:regBuffer error:&error];
    [regBuffer setFrameLength:beatLength];
    [audioFileFirstClick readIntoBuffer:firstBuffer error:&error];
    [firstBuffer setFrameLength:beatLength];
    
    int counter = 0;
    for(int k = 0; k < [[audioFileMainClick processingFormat] channelCount]; k++){
        for (int i = 0; i < sizeOfArr; i++){
            //laghu
            if(thala[i] == 1){
                int j = 0;
                while(j != laghuCount){
                    if(j == 0){
                        memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], firstBuffer.floatChannelData[k], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
                        counter += 1;
                        memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], firstBuffer.floatChannelData[k], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
                        counter += 1;
                    }
                    else{
                        memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], regBuffer.floatChannelData[k], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
                        counter += 1;
                        memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], regBuffer.floatChannelData[k], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
                        counter += 1;
                    }
                    j+= 1;
                }
            }
            //anudrutham
            else if(thala[i] == 0){
                memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], regBuffer.floatChannelData[k], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
                counter += 1;
                memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], regBuffer.floatChannelData[k], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
                counter += 1;
            }
            //drutham
            else if(thala[i] == 2){
                memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], firstBuffer.floatChannelData[k], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
                counter += 1;
                memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], firstBuffer.floatChannelData[k], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
                counter += 1;
                memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], regBuffer.floatChannelData[k], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
                counter += 1;
                memcpy(buffermainclick.floatChannelData[k] + [_sharedInstance mod: int((counter) * 48000 * 60.0 / bpm) -displacement :buffermainclick.frameLength], regBuffer.floatChannelData[k], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
                counter += 1;
            }
        }
    }
    
    return buffermainclick;
}

-(int) mod:(int)a :(int) b{
    int ret = a % b;
    if(ret < 0)
        ret+=b;
    return ret;
}
-(AVAudioPCMBuffer*)generateRupakaBuffer:(float) bpm dis:(int) displacement{
    NSError *error = nil;
    [audioFileMainClick setFramePosition:0];
    [audioFileFirstClick setFramePosition:0];
    
    int beatLength = 48000 * 60.0 / bpm;
    AVAudioPCMBuffer *regBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:beatLength];

    
    AVAudioPCMBuffer *rupakaBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:int(3.0 * 48000 * 60.0 / bpm)];
    
    AVAudioPCMBuffer *firstBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileFirstClick.processingFormat frameCapacity:beatLength];
    
    
    rupakaBuffer.frameLength = int(3.0 * 48000 * 60.0 / bpm);
    
    [audioFileMainClick readIntoBuffer:regBuffer error:&error];
    [regBuffer setFrameLength:beatLength];
    [audioFileFirstClick readIntoBuffer:firstBuffer error:&error];
    [firstBuffer setFrameLength:beatLength];
    
//    NSLog(@"%d", firstBuffer.frameLength);
//    NSLog(@"%d", regBuffer.frameLength);
    for(int i = 0; i < [[audioFileFirstClick processingFormat] channelCount]; i++){
        memcpy(rupakaBuffer.floatChannelData[i] + [_sharedInstance mod: -displacement :rupakaBuffer.frameLength], firstBuffer.floatChannelData[i], firstBuffer.frameLength* firstBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(rupakaBuffer.floatChannelData[i] + [_sharedInstance mod:(regBuffer.frameLength - displacement) :rupakaBuffer.frameLength], regBuffer.floatChannelData[i], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(rupakaBuffer.floatChannelData[i] +  [_sharedInstance mod:(int(2.0 * 48000 * 60.0 / bpm) - displacement) :rupakaBuffer.frameLength], regBuffer.floatChannelData[i], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
    }
    return rupakaBuffer;
}

-(AVAudioPCMBuffer*)generateKhandaBuffer:(float) bpm dis:(int) displacement{
    NSError *error = nil;
    [audioFileMainClick setFramePosition:0];
    [audioFileFirstClick setFramePosition:0];
    
    int beatLength = 48000 * 60.0 / bpm;
    AVAudioPCMBuffer *regBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:beatLength];

    
    AVAudioPCMBuffer *khandaBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:int(2.5 * 48000 * 60.0 / bpm)];
    
    AVAudioPCMBuffer *firstBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileFirstClick.processingFormat frameCapacity:beatLength];
    
    
    khandaBuffer.frameLength = int(2.5 * 48000 * 60.0 / bpm);
    
    [audioFileMainClick readIntoBuffer:regBuffer error:&error];
    [regBuffer setFrameLength:beatLength];
    [audioFileFirstClick readIntoBuffer:firstBuffer error:&error];
    [firstBuffer setFrameLength:beatLength];
    
    for(int i = 0; i < [[audioFileMainClick processingFormat] channelCount]; i++){
        memcpy(khandaBuffer.floatChannelData[i] + [_sharedInstance mod: -displacement :khandaBuffer.frameLength], firstBuffer.floatChannelData[i], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(khandaBuffer.floatChannelData[i] + [_sharedInstance mod:(regBuffer.frameLength - displacement) :khandaBuffer.frameLength], regBuffer.floatChannelData[i] + int(1500*pow(bpm/75.0,0.5)), int(regBuffer.frameLength/2.0) * regBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(khandaBuffer.floatChannelData[i] +  [_sharedInstance mod:(int(1.5 * 48000 * 60.0 / bpm) - displacement) :khandaBuffer.frameLength], regBuffer.floatChannelData[i], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
    }
    return khandaBuffer;
}

-(AVAudioPCMBuffer*)generateMisraBuffer:(float) bpm dis:(int) displacement{
    NSError *error = nil;
    [audioFileMainClick setFramePosition:0];
    [audioFileFirstClick setFramePosition:0];
    
    int beatLength = 48000 * 60.0 / bpm;
    AVAudioPCMBuffer *regBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:beatLength];

    AVAudioPCMBuffer *firstBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileFirstClick.processingFormat frameCapacity:beatLength];
    
    
    AVAudioPCMBuffer *misraBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:int(3.5 * 48000 * 60.0 / bpm)];
    misraBuffer.frameLength = int(3.5 * 48000 * 60.0 / bpm);
    
    [audioFileMainClick readIntoBuffer:regBuffer error:&error];
    [regBuffer setFrameLength:beatLength];
    [audioFileFirstClick readIntoBuffer:firstBuffer error:&error];
    [firstBuffer setFrameLength:beatLength];
    
    for(int i = 0; i < [[audioFileMainClick processingFormat] channelCount]; i++){
        memcpy(misraBuffer.floatChannelData[i] + [_sharedInstance mod:-displacement :misraBuffer.frameLength], regBuffer.floatChannelData[i] + int(1500*pow(bpm/75.0,0.5)), int(0.5 * regBuffer.frameLength) * regBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(misraBuffer.floatChannelData[i] + [_sharedInstance mod:(int(0.5 * 48000 * 60.0 / bpm) -displacement) :misraBuffer.frameLength], regBuffer.floatChannelData[i], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(misraBuffer.floatChannelData[i] + [_sharedInstance mod:(int(1.5 * 48000 * 60.0 / bpm) -displacement) :misraBuffer.frameLength] , firstBuffer.floatChannelData[i], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(misraBuffer.floatChannelData[i] +  [_sharedInstance mod:(int(2.5 * 48000 * 60.0 / bpm) - displacement) :misraBuffer.frameLength], firstBuffer.floatChannelData[i], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
    }
    return misraBuffer;
}

-(AVAudioPCMBuffer*)generateSankeernaBuffer:(float) bpm dis:(int) displacement{
    NSError *error = nil;
    [audioFileMainClick setFramePosition:0];
    [audioFileFirstClick setFramePosition:0];
    
    int beatLength = 48000 * 60.0 / bpm;
    AVAudioPCMBuffer *regBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:beatLength];
    
    AVAudioPCMBuffer *firstBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileFirstClick.processingFormat frameCapacity:beatLength];

    
    AVAudioPCMBuffer *sankeernaBuffer = [[AVAudioPCMBuffer alloc] initWithPCMFormat:audioFileMainClick.processingFormat frameCapacity:int(4.5 * 48000 * 60.0 / bpm)];
    sankeernaBuffer.frameLength = int(4.5 * 48000 * 60.0 / bpm);
    
    [audioFileMainClick readIntoBuffer:regBuffer error:&error];
    [regBuffer setFrameLength:beatLength];
    [audioFileFirstClick readIntoBuffer:firstBuffer error:&error];
    [firstBuffer setFrameLength:beatLength];
    
    
    for(int i = 0; i < [[audioFileMainClick processingFormat] channelCount]; i++){
        memcpy(sankeernaBuffer.floatChannelData[i] + [_sharedInstance mod: -displacement :sankeernaBuffer.frameLength], firstBuffer.floatChannelData[i], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(sankeernaBuffer.floatChannelData[i] + [_sharedInstance mod:(regBuffer.frameLength - displacement) :sankeernaBuffer.frameLength], regBuffer.floatChannelData[i], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(sankeernaBuffer.floatChannelData[i] +  [_sharedInstance mod:int(2.0*48000 * 60.0 / bpm) - displacement :sankeernaBuffer.frameLength], firstBuffer.floatChannelData[i], firstBuffer.frameLength * firstBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(sankeernaBuffer.floatChannelData[i] + [_sharedInstance mod:int(3.0*48000 * 60.0 / bpm) - displacement :sankeernaBuffer.frameLength], regBuffer.floatChannelData[i] + int(1500*pow(bpm/75.0,0.5)), int(regBuffer.frameLength/2.0) * regBuffer.format.streamDescription->mBytesPerFrame);
        memcpy(sankeernaBuffer.floatChannelData[i] +  [_sharedInstance mod:int(3.5*48000 * 60.0 / bpm) - displacement :sankeernaBuffer.frameLength], regBuffer.floatChannelData[i], regBuffer.frameLength * regBuffer.format.streamDescription->mBytesPerFrame);
        
    }
    return sankeernaBuffer;
}

-(void)playSound:(float) speed :(NSString*) tag :(int)khandaCount :(int)misraCount :(int)sankeernaCount :(int[]) thalas :(int) sizeOfArr :(int) numBeats :(int)otherCount{
    isStopped = false;
    loopcount = 0;
    currSpeed = speed;
    dispatch_time_t delay;
    
    if(!([tag isEqualToString:@"KhandaTag"] || [tag isEqualToString:@"MisraTag"] || [tag isEqualToString:@"SankeernaTag"])){
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.3*pow((75.0/speed), 1)*NSEC_PER_SEC);
    }
    else{
        delay = dispatch_time(DISPATCH_TIME_NOW, 0.2*pow((75.0/speed), 2)*NSEC_PER_SEC);
    }
    
    dispatch_after(delay, dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0), ^{
        AVAudioPCMBuffer *buffer;
        if(khandaCount == 1){
//            NSLog(@"Khanda1");
            buffer = [_sharedInstance generateKhandaBuffer:speed dis:0];
        }
        else if(khandaCount == 2){
//            NSLog(@"Khanda2");
            buffer = [_sharedInstance generateKhandaBuffer:speed dis:int(48000 * 60.0 / speed)];
        }
        else if(khandaCount == 3){
//            NSLog(@"Khanda3");
            buffer = [_sharedInstance generateKhandaBuffer:speed dis:int(1.5 * 48000 * 60.0 / speed)];
        }
        else if(misraCount == 1){
//            NSLog(@"Misra1");
            buffer = [_sharedInstance generateMisraBuffer:speed dis:0];
        }
        else if(misraCount == 2){
//            NSLog(@"Misra2");
            buffer = [_sharedInstance generateMisraBuffer:speed dis:int(0.5*48000 * 60.0 / speed)];
        }
        else if(misraCount == 3){
//            NSLog(@"Misra3");
            buffer = [_sharedInstance generateMisraBuffer:speed dis:int(1.5 * 48000 * 60.0 / speed)];
        }
        else if(misraCount == 4){
//            NSLog(@"Misra4");
            //have to multiply float before due to loss of precision
            buffer = [_sharedInstance generateMisraBuffer:speed dis:int(2.5 * 48000 * 60.0 / speed)];
        }
        else if(sankeernaCount == 1){
//            NSLog(@"Sankeerna1");
            buffer = [_sharedInstance generateSankeernaBuffer:speed dis:0];
        }
        else if(sankeernaCount == 2){
            buffer = [_sharedInstance generateSankeernaBuffer:speed dis:int(48000 * 60.0 / speed)];
        }
        else if(sankeernaCount == 3){
            buffer = [_sharedInstance generateSankeernaBuffer:speed dis:int(2.0 *48000 * 60.0 / speed)];
        }
        else if(sankeernaCount == 4){
            buffer = [_sharedInstance generateSankeernaBuffer:speed dis:int(3.0 * 48000 * 60.0 / speed)];
        }
        else if(sankeernaCount == 5){
            buffer = [_sharedInstance generateSankeernaBuffer:speed dis:int(3.5 * 48000 * 60.0 / speed)];
        }
        else if([tag isEqualToString:@"Rupakam"]){
            buffer = [_sharedInstance generateRupakaBuffer:speed dis:int((otherCount -1) *48000.0 * 60.0 / speed)];
        }
        else if([tag isEqualToString:@"DoubleKalai"]){
            buffer = [_sharedInstance generateDoubleBuffer:speed:thalas:sizeOfArr:int((otherCount -1) *48000.0 * 60.0 / speed):numBeats];
        }
        else{
            buffer = [_sharedInstance generateBuffer:speed:thalas:sizeOfArr:int((otherCount -1) *48000.0 * 60.0 / speed):numBeats];
        }
        if(!isStopped){
            if(audioEngine.isRunning){
                if ([audioPlayerNode isPlaying]) {
                    [audioPlayerNode scheduleBuffer:buffer atTime:nil options:AVAudioPlayerNodeBufferInterrupts completionHandler:nil];
                } else {
                    [audioPlayerNode play];
                }
                [audioPlayerNode scheduleBuffer:buffer atTime:nil options: AVAudioPlayerNodeBufferLoops completionHandler:nil];
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
    isStopped = true;
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

-(void) stopEngine{
    [audioEngine pause];
    [audioPlayerNode stop];
}

-(void) restartEngine{
    [audioEngine startAndReturnError:nil];
}

-(void) showNativeAlert: (NSString*) title :(NSString*) msg :(NSString*) b1
{
    UIAlertController *alertController = [UIAlertController alertControllerWithTitle:title message:msg preferredStyle:UIAlertControllerStyleAlert];
    UIAlertAction *okAction = [UIAlertAction actionWithTitle:b1 style:UIAlertActionStyleDefault handler:^(UIAlertAction * _Nonnull action) {
    }];
    [alertController addAction:okAction];
    [[[[UIApplication sharedApplication] keyWindow] rootViewController] presentViewController:alertController animated:YES completion:nil];
}


@end

extern "C"
{
    void stopAudioEngine(){
        [[SoundPlayer sharedInstance] stopEngine];
    }
    void restartAudioEngine(){
        [[SoundPlayer sharedInstance] restartEngine];
    }
    void InitSPlayer(char* otherSound, char* firstSound){
        [[SoundPlayer sharedInstance] initSoundPlayer:[NSString stringWithUTF8String:otherSound]:[NSString stringWithUTF8String:firstSound]];
    }

    void showNativeAlert(char* title, char *msg, char *b1){
        [[SoundPlayer sharedInstance] showNativeAlert:[NSString stringWithUTF8String:title] :[NSString stringWithUTF8String:msg]  :[NSString stringWithUTF8String:b1] ];
    }
    void IOSMuteSound(){
        [[SoundPlayer sharedInstance] muteSound];
    }
    void IOSUnMuteSound(){
        [[SoundPlayer sharedInstance] unmuteSound];
    }
    void IOSStopSound(){
        [[SoundPlayer sharedInstance] stopSound];
    }
    void IOSPlaySound(float speed, char* tag, int khandaCount, int misraCount, int sankeernaCount, int thalas[], int sizeOfArr, int numBeats, int otherCount){
        [[SoundPlayer sharedInstance] playSound:speed :[NSString stringWithUTF8String:tag]:khandaCount:misraCount:sankeernaCount:thalas:sizeOfArr:numBeats:otherCount];
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

