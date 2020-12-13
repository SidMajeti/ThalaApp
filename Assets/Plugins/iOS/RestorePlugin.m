//
//  RestorePlugin.m
//  
//
//  Created by Sid Majeti on 11/25/20.
//

#import <Foundation/Foundation.h>


void iCloudKV_Synchronize() {
    [[NSUbiquitousKeyValueStore defaultStore] synchronize];
}

void iCloudKV_SetLong(char * key, long value) {
    [[NSUbiquitousKeyValueStore defaultStore] setObject:[NSNumber numberWithLong:value] forKey:[NSString stringWithUTF8String:key]];

}

void iCloudKV_SetFloat(char * key, float value) {
    [[NSUbiquitousKeyValueStore defaultStore] setObject:[NSNumber numberWithFloat:value] forKey:[NSString stringWithUTF8String:key]];
}

long iCloudKV_GetLong(char * key) {
    NSNumber * num = (NSNumber *)([[NSUbiquitousKeyValueStore defaultStore] objectForKey:[NSString stringWithUTF8String:key]]);
    long i = 0;
    if (num != nil)
        i = [num longValue];
    return i;
}

float iCloudKV_GetFloat(char * key) {
    NSNumber * num = (NSNumber *)([[NSUbiquitousKeyValueStore defaultStore] objectForKey:[NSString stringWithUTF8String:key]]);
    float i = 0;
    if (num != nil)
        i = [num floatValue];
    return i;
}

void iCloudKV_Reset() {
    NSUbiquitousKeyValueStore *kvStore = [NSUbiquitousKeyValueStore defaultStore];
    NSDictionary *kvd = [kvStore dictionaryRepresentation];
    NSArray *arr = [kvd allKeys];
    for (int i=0; i < arr.count; i++){
        NSString *key = [arr objectAtIndex:i];
        [kvStore removeObjectForKey:key];
    }
    [[NSUbiquitousKeyValueStore defaultStore] synchronize];
}
