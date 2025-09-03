USE DemoForum;
GO

-- 假使用者（密碼皆為 "1234"，SHA256 為 03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4）
INSERT INTO Users (UserName, PasswordHash, Email, DisplayName) VALUES
(N'haha', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'haha@test.com', N'哈'),
(N'pickpick', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'pick@test.com', N'皮克'),
(N'guest', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'guest@test.com', N'訪客');

-- 假主題
INSERT INTO Topics (UserID, Title, Content) VALUES
(1, N'第一篇文章', N'這是第一篇文章的內容。'),
(2, N'ASP.NET Web Forms 討論', N'大家覺得 Web Forms 好用嗎？');

-- 假回覆
INSERT INTO Replies (TopicID, UserID, Content) VALUES
(1, 2, N'哈哈，不錯喔'),
(1, 3, N'我是訪客的留言'),
(2, 1, N'我覺得挺方便的'); 