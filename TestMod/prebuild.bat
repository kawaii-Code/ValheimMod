tasklist | find /i "valheim.exe" && taskkill /im valheim.exe /F

xcopy /Y "C:\DATA\VALHEIM\ValheimRipped\Assets\AssetBundles" "AssetBundles"
DEL "AssetBundles\*.meta" "AssetBundles\*.manifest"
