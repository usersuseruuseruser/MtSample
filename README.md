Замечания касательно паттерна inbox/outbox.

**Проблема**: Если consumer A захочет сделать запись в бд о чем-нибудь и опубликовать сообщение о том что запись произошла, то возможна неконсистентность системы: представим себе ситуацию,
где мы сначала сохраняем запись в бд, а потом публикуем ее (назовем это а).

а.1) Запись в бд сохранилась, но опубликовать событие мы не смогли(брокер не отвечает). Тогда следующие сервисы не узнают об этом событии, что и создает неконсистентность

а.2) Запись в бд не сохранилась(сервер субд не отвечает). Тогда мы и не публикуем событие, делая ретраи или доставляя его заново в очередь. Вполне нормальный исход, но он не избавляет нас
от проблемы, ведь в итоге при обработке сообщения заново мы придем к пунктам а.1, б.1, б.2

Теперь посмотрим на случай, когда мы сначала публикуем событие об обработке сообщения, а потом его сохраняем в бд(назовем это б)

б.1) Сообщение опубликовалось, но сервер субд не отвечает, так что мы не знаем сохранилась ли запись или нет. Неконсистентность - что сообщение об успешной обработке уже опубликовано

б.2) Сообщение не опубликовалось, тогда мы и не сохраняем запись(зачем не сохраняем? чтобы не делать неконсистентность - если сохраним, а брокер не ответит, то ретраи обработки этого сообщения
столкнутся с тем, что эм у них неконсистентность в информации). Аналогично а.2 приходим к выводу что нужно делать ретраи или ределивери, приходя к а.1, а.2, б.1

Нет, я не создаю иллюзию что неконсистентность будет всегда: если лег субд или не дай бог брокер, это вообще огромный провал, такое бывает редко и чаще всего и запись, и сообщение отправляются
без сбоев. Однако! Если что-то может произойти, то оно произойдет, и если оно произойдет, то обязательно с неконсистентностью. Как фиксить? Нам нужно делать запись в бд и публикацию сообщения
в транзакции - тогда 100% все будет оки. Как сделать между ними транзакцию? Никак. Они никак не связаны. Но! на помощь приходит Outbox Pattern

![0_WUdjvJ6zsVqaKo8p](https://github.com/user-attachments/assets/f31067f3-06d8-42b1-a684-4d19dc86c82d)

Очень просто: вместе с сохранением записи, в той же транзакции, мы сохраняем в outbox таблицу запись о том, что нужно опубликовать в брокер сообщение об обработке сообщения. Потом какой-то
воркер(он может быть в том же проекте, а может и нет) время от времени мониторит эту outbox таблицу и отправляет сообщение в брокер. Это гарантирует, что событие обработки заказа не пропадет
при отказе брокера или бд - оно 100% или сохранится с записью в бд, или нет - неконсистентности нет. 

Здесь, однако, есть некоторые дополнительные требования к реализации дальнейших консюмеров. К примеру, воркер, доставляющий сообщения из outbox в брокер, может безошибочно доставить его,
но при удалении сообщения из outbox(задача этой таблицы - только говорить, что нужно доставитьв брокер вот это сообщение. если оно доставлено, то запись можно удалять) умрет сервер субд где хранится outbox. 
Тогда сообщение будет доставлено, но воркер не сможет удалить его. В общем,
насколько я понял, это никак не решается. Это буквально особенность реализации outbox: она гарантирует at-least-once delivery, то есть хотя бы один раз событие опубликуется в брокер.
Так вот, получается, событие(скажет отправка емейла о созаднии юзера) может попасть в брокер два-три-четыре(очень очень маловероятно, но может). Чтобы потребитель не отправил этот емеил 4 раза,
он должен быть **идепотентным**, то есть как-то следить за тем, что при одинаковых входных данных(сообщении) он отрабатывал только один раз. Это уже не относится к outbox, ведь это
классическая задача микросервисов: микросервис отработал, но брокер умер и ack не дошел. Заработав, брокер заставит консьюмера отработать заново, но нужно ли это? НЕТ. Он уже отработал. Потребитель
должен как-то следить за тем, чтобы при одинаковых сообщениях он отрабатывал только один раз. 

Для облегчения этой реализации будем в купе с outbox использовать **inbox**. 
![2020-12-30-outbox](https://github.com/user-attachments/assets/18fd0aad-0706-4580-9bd0-48bfe124eef9)

Теперь брокер будет доставлять сообщения не сразу потребителю, а сначала сохранять их в таблицу сообщений на стороне микросервиса-потребителя. Помимо самого сообщения и id, он еще и сохраняет
статус его обработки и timestamp(тут говорят про то что это помогает обрабатывать их последовательно, но чет не вдавался в это). Так вот, если у нас брокер содержит 4 одинаковых сообщения
об отправке емейла, то, консюмер получив их и сохранив в inbox, сохранит только одно, а другие три получат исключение о том что оно уже сохранено(в реализации MassTransit это исключение
хорошее, то есть все 3 акнутся и пропадут из очереди брокера как решенные). Отлично! теперь у нас только одно - **exactly once** - сообщение, которое нужно обработать консюмеру. Однако,
проблема обрабокти этого сообщения консьюмером несколько раз никуда не девается: представим консьюмер начал обработку(message status = processing). отправил сообщение. а вот message status
= processed он сделать не смог - внезапно инбокс не работает. в итоге в инбоксе статус сообщения processing, хотя оно уже обработалось. что делать с таким сообщением? дальше я уже плыву
в этом вопросе, ведь инфы об этом крайне мало. если мы настроим политику так, что status = processing с определенным таймаутом попадает в консюмера заново, то он может отправить емеил дважды
. С другой стороны, если бы он не отправлял емеил а сохранял сущность в бд, то идепотентность делается несложно - транзакция в обе таблицы. Я так понимаю, тут уже все зависит от случая к случаю
в случае с емейлом, если на processing таймаут, я совершенно не знаю как мне узнать отправился емеил или нет, обрабатывать сообщение или нет. p.s. в чате сказали что никак и нельзя, емеил это 
типо особый случай, идемпотентности тут никак не сделать. 
p.s. я не понимаю как сделать идемпотентность и без отправки емейла. в mass transit сообщение удаляется из inbox после отработки consumer'a. а что если консюмер сделал запись в бд, но в inbox удаление не произошло? я не совсем понимаю как совместить удаление сообщения из inbox и запись в бд в одну транзакцию, ведь mass transit это обрабатывает за кулисами. гм.

полезные ссылки:

[ламода](https://habr.com/ru/companies/lamoda/articles/678932/) рассказывает как они использовали этот паттерн

[ютубер](https://youtu.be/032SfEBFIJs?si=MC0OL1dIWzA7va-9) рассказывает об этом паттерне, как он реализован в MassTransit. Можно смотреть не только .net-разработчикам, он говорит абстракциями и лишь иногда показывает в коде как работает. обязательно читайте комменты! он забыл упомянуть об inbox, хотя использовал его. видео может сбить вас с толку, если не обратитесь к комментаторам
