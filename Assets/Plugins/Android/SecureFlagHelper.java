package com.hkpolice.unityapp;

import android.app.Activity;
import android.view.WindowManager.LayoutParams;

public class SecureFlagHelper {
    public static void setFlagSecure(Activity activity) {
        activity.getWindow().setFlags(LayoutParams.FLAG_SECURE, LayoutParams.FLAG_SECURE);
    }
}