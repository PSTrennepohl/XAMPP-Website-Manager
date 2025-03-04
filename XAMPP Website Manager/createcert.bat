@echo off

set OPENSSL_CONF=%1\apache\conf\openssl.cnf

%1\apache\bin\openssl req -new -newkey rsa:4096 -days 365 -nodes -x509 -subj "/C=%2/ST=%3/L=%4/O=%5/CN=%6" -keyout %7.key -out %7.cert

echo 
echo success