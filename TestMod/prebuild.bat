tasklist | find /i "valheim.exe" && taskkill /im valheim.exe /F

xcopy /Y "..\..\ValheimRipped\Assets\AssetBundles\" "AssetBundles"
rm "AssetBundles\*.meta" "AssetBundles\*.manifest"
