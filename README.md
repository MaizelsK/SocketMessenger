# SocketMessenger
Клиент написал Таир  
Сервер написал Кирилл
--
Создать клиент-серверное приложение чат. Чат будет работать ТОЛЬКО ДЛЯ ТРЁХ клиентов. 
Суть проста, клиент построен на постоянном соединении с сервером. 
Когда клиент подключается к серверу, все уже подключенные клиенты об этом узнают (Пользователь N заходит в чат). 
Отправленное сообщение любым клиентом видно другим клиентам.
Сообщения передавать в формате json (от кого, сообщение).

Клиент отправляет данные серверу, сервер рассылает их всем клиентам.