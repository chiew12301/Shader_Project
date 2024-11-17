#import <Foundation/Foundation.h>

@interface ScreenshotDetector : NSObject

+ (void)startObservingScreenshotNotification;
+ (void)stopObservingScreenshotNotification;

@end
