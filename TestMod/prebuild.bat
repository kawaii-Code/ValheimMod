tasklist | find /i "valheim.exe" && taskkill /im valheim.exe /F

xcopy /Y "D:\dev\repos\ValheimRipped\Assets\AssetBundles" "AssetBundles"
DEL "AssetBundles\*.meta" "AssetBundles\*.manifest"
