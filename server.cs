//basically robbed this from BLG default prefs
//https://forum.blockland.us/index.php?topic=320521.0 heres a good reference
function WV_registerPref(%cat, %title, %type, %variable, %addon, %default, %params, %callback, %legacy, %isSecret, %isHostOnly) {
    new ScriptObject(Preference)
    {
        className     = "SWV_preference";

        addon         = %addon;
        category      = %cat;
        title         = %title;

        type          = %type;
        params        = %params;

        variable      = %variable;

        defaultValue  = %default;

        hostOnly      = %isHostOnly;
        secret        = %isSecret;

        loadNow        = false; // load value on creation instead of with pool (optional)
        noSave         = false; // do not save (optional)
        requireRestart = false; // denotes a restart is required (optional)
    };
}
//kinda robbed from script_blizzard
function SWV_preference::onUpdate(%this)
{
	RainWind_update();
}

function RainWind_update()
{
	//yes set percentaNge, good typo torque game engine
	Rain.setPercentange($Pref::WindRain::RainMultiplier);
	if($Pref::WindRain::RainNumDrops > -1)
		Rain.numDrops = $Pref::WindRain::RainNumDrops;
	else Rain.numDrops = $Rain::numDrops;
	
	//function should just be named sendUpdate
	Rain.inspectPostApply();
	
	if($Pref::WindRain::WindEnabled)
	{
		$Pref::WindRain::WindVelocity = VectorAdd($Pref::WindRain::WindVelocity, "0 0 0"); //make sure its a vector3
		Sky.setWindVelocity(getWord($Pref::WindRain::WindVelocity, 0), getWord($Pref::WindRain::WindVelocity, 1), getWord($Pref::WindRain::WindVelocity, 2));
		Sky.windEffectPrecipitation = $Pref::WindRain::WindEffectRainEnabled;
	}
	Sky.sendUpdate();
}

package Script_SetWindVelocityPackage
{
	//update the rain on skybox change
	function setSkyBox (%filename)
	{
		%r = parent::setSkyBox (%filename);
		if(isObject(rain))
			RainWind_Update();
			
		return %r;
	}
};
activatePackage(Script_SetWindVelocityPackage);
resetAllOpCallFunc();

//super hacky for debug
//if(isObject(ScriptSetWindVelocityPrefs))
//	ScriptSetWindVelocityPrefs.delete();
//
//for(%I = PreferenceGroup.getcount() - 1; %I >= 0; %I--)
//{
//	%t = PreferenceGroup.getObject(%I);
//	if(%t.addon $= "Script_SetWindVelocity")
//	{
//		talk(%i);
//		%t.delete();
//	}
//}

if(!isObject(ScriptSetWindVelocityPrefs))
{
    registerPreferenceAddon("Script_SetWindVelocity", "Wind & Rain Options", "weather_clouds");

    //general regen settings
    WV_registerPref("Wind Options", "Wind Enabled"        			 , "bool"   , 	 "$Pref::WindRain::WindEnabled"      	    , "Script_SetWindVelocity", 0, "");
	WV_registerPref("Wind Options", "Wind Velocity (vec3f)"          , "string" , 	 "$Pref::WindRain::WindVelocity"      	    , "Script_SetWindVelocity", 0, "128 1");
	
	WV_registerPref("Rain Options", "Wind Effects Precipitation"     , "bool"   , 	 "$Pref::WindRain::WindEffectRainEnabled"   , "Script_SetWindVelocity", 0, "");
    WV_registerPref("Rain Options", "Rain Multiplier"     			 , "num"    , 	 "$Pref::WindRain::RainMultiplier"          , "Script_SetWindVelocity", 1, "0.001 100 0.001");
	WV_registerPref("Rain Options", "Rain Num Drops (-1 for default)", "num"    , 	 "$Pref::WindRain::RainNumDrops"          , "Script_SetWindVelocity",   -1, "-1 100000 1");
}