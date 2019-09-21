makecert -r -pe -n "CN=Hook Test CA" -ss CA -sr CurrentUser -a sha256 -cy authority -sky signature -sv HookTestCA.pvk HookTestCA.cer

makecert -pe -n "CN=Hook Test SPC" -a sha256 -cy end -sky signature -ic HookTestCA.cer -iv HookTestCA.pvk  -sv HookTestSPC.pvk HookTestSPC.cer

pvk2pfx -pvk HookTestSPC.pvk -spc HookTestSPC.cer -pfx HookTestSPC.pfx


pause