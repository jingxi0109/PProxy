dotnet publish -c Release -o out -r linux-x64
tar -zcvf  pub.tar.gz out/
smbclient -U administrator%Developer200  //192.168.7.16/storfiles <<- EOF
put pub.tar.gz  pub.tar.gz 
exit
EOF
