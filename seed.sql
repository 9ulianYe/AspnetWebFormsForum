USE DemoForum;
GO

-- ���ϥΪ̡]�K�X�Ҭ� "1234"�ASHA256 �� 03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4�^
INSERT INTO Users (UserName, PasswordHash, Email, DisplayName) VALUES
(N'haha', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'haha@test.com', N'��'),
(N'pickpick', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'pick@test.com', N'�֧J'),
(N'guest', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', 'guest@test.com', N'�X��');

-- ���D�D
INSERT INTO Topics (UserID, Title, Content) VALUES
(1, N'�Ĥ@�g�峹', N'�o�O�Ĥ@�g�峹�����e�C'),
(2, N'ASP.NET Web Forms �Q��', N'�j�aı�o Web Forms �n�ζܡH');

-- ���^��
INSERT INTO Replies (TopicID, UserID, Content) VALUES
(1, 2, N'�����A������'),
(1, 3, N'�ڬO�X�Ȫ��d��'),
(2, 1, N'��ı�o����K��'); 