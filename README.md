# gas-station-api
## Запуск с помощью Docker
### Шаг 1. Задайте конфигурацию EmailOptions

В секции `EmailOptions` файла `~/src/backend/API/appsettings.json` укажите почту отправителя писем (`SenderEmail`), пароль от данной почты (`Password`), адрес SMTP-сервера (`SmtpServer`), порт, используемый SMTP-сервером (`Port`). Прочие параметры опциональны к переопределению. О том, как использовать Gmail для отправки писем см.: https://support.google.com/a/answer/176600

```json
"EmailOptions": {  
  "SenderEmail": "SenderEmail",  
  "SenderName": "SenderName",  
  "SmtpServer": "smtp.example.com",  
  "Port": 465,  
  "UseSsl" : "true",  
  "UserName" : "UserName",
  "Password": "***"  
}
```
### Шаг 2. Настройте TLS/SSL сертификат

В файле `.env` задайте переменную окружения `CERTIFICATE_DIRECTORY`, указав путь к директории, содержащей сертификат. Задайте пароль от сертификата в `ASPNETCORE_Kestrel__Certificates__Default__Password`.  Замените название файла сертификата (`aspnetapp.pfx`) в переменной `ASPNETCORE_Kestrel__Certificates__Default__Path` на ваш сертификат

### Шаг 3. Соберите и запустите проект

```
docker compose up
```
