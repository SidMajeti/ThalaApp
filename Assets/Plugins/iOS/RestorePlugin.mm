//
//  RestorePlugin.m
//
//
//  Created by Sid Majeti on 11/25/20.
//

#import <AVFoundation/AVFoundation.h>


@interface RestorePlugin : NSObject

@end

@implementation RestorePlugin

static RestorePlugin *_sharedInstance;

+(RestorePlugin*) sharedInstance
{
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        _sharedInstance = [[RestorePlugin alloc] init];
    });
    return _sharedInstance;
}

-(id) init
{
    self = [super init];
    return self;
}

-(bool)iCloudKV_Synchronize{
    return [[NSUbiquitousKeyValueStore defaultStore] synchronize];
}

-(void) iCloudKV_SetLong: (NSString *)key :(long) value {
    [[NSUserDefaults standardUserDefaults] setObject:[NSNumber numberWithLong:value] forKey:key];
    
    [[NSUbiquitousKeyValueStore defaultStore] setObject:[NSNumber numberWithLong:value] forKey:key];
    
    [[NSNotificationCenter defaultCenter]addObserver:self selector:@selector(newCloudData:) name:NSUbiquitousKeyValueStoreDidChangeExternallyNotification object:nil];
    
}

-(void) newCloudData:(NSNotification*) notification{
//    NSLog(@"ChangedValue");
}

-(long) iCloudKV_GetLong :(NSString *)key {
    NSNumber * num = (NSNumber *)([[NSUbiquitousKeyValueStore defaultStore] objectForKey:key]);
    long i = 0;
    if (num != nil)
        i = [num longValue];
    else{
        num = (NSNumber *)([[NSUserDefaults standardUserDefaults] objectForKey:key]);
//        NSLog(@"Valueofnum : %@" , num);
        i = [num longValue];
    }
    return i;
}

-(void) iCloudKV_Reset{
    NSUbiquitousKeyValueStore *kvStore = [NSUbiquitousKeyValueStore defaultStore];
    NSDictionary *kvd = [kvStore dictionaryRepresentation];
    NSArray *arr = [kvd allKeys];
    for (int i=0; i < arr.count; i++){
        NSString *key = [arr objectAtIndex:i];
        [kvStore removeObjectForKey:key];
    }
    [[NSUbiquitousKeyValueStore defaultStore] synchronize];
}

@end


extern "C"
{
    void iCloudKV_Reset(){
        [[RestorePlugin sharedInstance] iCloudKV_Reset];
    }
    long iCloudKV_GetLong(char* key){
        return [[RestorePlugin sharedInstance] iCloudKV_GetLong:[NSString stringWithUTF8String:key]];
    }
    bool iCloudKV_Synchronize(){
        return [[RestorePlugin sharedInstance] iCloudKV_Synchronize];
    }
    void iCloudKV_SetLong(char* key, long value){
        [[RestorePlugin sharedInstance] iCloudKV_SetLong:[NSString stringWithUTF8String:key] :value];
    }
}



