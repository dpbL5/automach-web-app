-- AutoMach Game Store Seed Data
-- Tạo dữ liệu mẫu: 20 games, 11 accounts (10 users + 1 admin), tags và hình ảnh

-- USE [AutoMachDB]; -- Thay đổi tên database nếu cần

-- Xóa dữ liệu cũ (nếu có)
DELETE FROM GameTags;
DELETE FROM ImageUrls;
DELETE FROM Reviews;
DELETE FROM TransactionItems;
DELETE FROM Transactions;
DELETE FROM GamesOwned;
DELETE FROM Games;
DELETE FROM Tags;
DELETE FROM Accounts;

-- Reset Identity columns
DBCC CHECKIDENT ('Games', RESEED, 0);
DBCC CHECKIDENT ('Accounts', RESEED, 0);
DBCC CHECKIDENT ('Tags', RESEED, 0);
DBCC CHECKIDENT ('ImageUrls', RESEED, 0);
DBCC CHECKIDENT ('Transactions', RESEED, 0);
DBCC CHECKIDENT ('TransactionItems', RESEED, 0);
DBCC CHECKIDENT ('Reviews', RESEED, 0);

-- Seed Tags
INSERT INTO Tags (Title) VALUES
('Action'),
('Adventure'),
('RPG'),
('Strategy'),
('Simulation'),
('Sports'),
('Racing'),
('Horror'),
('Puzzle'),
('Indie'),
('Multiplayer'),
('Single Player'),
('Open World'),
('Fantasy'),
('Sci-Fi');

-- Seed Accounts (Mật khẩu đơn giản cho demo)
-- Admin Account
INSERT INTO Accounts (Name, Email, PhoneNumber, Username, Password, CreatedAt, Role) VALUES
(N'Admin User', 'admin@automach.com', '+84901234567', 'admin', '123456', GETDATE(), 'admin');

-- User Accounts
INSERT INTO Accounts (Name, Email, PhoneNumber, Username, Password, CreatedAt, Role) VALUES
(N'Nguyễn Văn An', 'an.nguyen@email.com', '+84912345678', 'anvannguyen', '123456', GETDATE(), 'user'),
(N'Trần Thị Bình', 'binh.tran@email.com', '+84923456789', 'binhtran', '123456', GETDATE(), 'user'),
(N'Lê Minh Cường', 'cuong.le@email.com', '+84934567890', 'cuongle', '123456', GETDATE(), 'user'),
(N'Phạm Thúy Dung', 'dung.pham@email.com', '+84945678901', 'dungpham', '123456', GETDATE(), 'user'),
(N'Hoàng Văn Em', 'em.hoang@email.com', '+84956789012', 'emhoang', '123456', GETDATE(), 'user'),
(N'Võ Thị Phương', 'phuong.vo@email.com', '+84967890123', 'phuongvo', '123456', GETDATE(), 'user'),
(N'Đặng Minh Quang', 'quang.dang@email.com', '+84978901234', 'quangdang', '123456', GETDATE(), 'user'),
(N'Bùi Thị Hà', 'ha.bui@email.com', '+84989012345', 'habui', '123456', GETDATE(), 'user'),
(N'Ngô Văn Ích', 'ich.ngo@email.com', '+84990123456', 'ichngo', '123456', GETDATE(), 'user'),
(N'Lý Thị Kim', 'kim.ly@email.com', '+84901234568', 'kimly', '123456', GETDATE(), 'user');

-- Seed Games (với nvarchar support cho tiếng Việt)
INSERT INTO Games (Title, Price, GameInfo, ReleaseDate, Developer, IsFeatured) VALUES
(N'Cyberpunk 2077', 59.99, N'Trò chơi nhập vai thế giới mở trong tương lai với cốt truyện phân nhánh và hành động đầy kịch tính. Khám phá Night City - thành phố tương lai đầy màu sắc với những công nghệ tiên tiến, những cuộc phiêu lưu không giới hạn và những lựa chọn có ý nghĩa. Trở thành V - một tay súng săn tiền thưởng trong hành trình tìm kiếm sự bất tử.', '2020-12-10', N'CD Projekt Red', 1),
(N'The Witcher 3: Wild Hunt', 39.99, N'Trò chơi nhập vai fantasy được đánh giá cao nhất mọi thời đại với thế giới mở rộng lớn và cốt truyện sâu sắc. Theo dõi Geralt of Rivia - thầy phù thủy huyền thoại trong cuộc hành trình tìm kiếm Ciri và khám phá những bí ẩn của thế giới phù thủy đầy ma thuật và quái vật.', '2015-05-19', N'CD Projekt Red', 1),
(N'Grand Theft Auto V', 29.99, N'Game hành động thế giới mở kinh điển với ba nhân vật chính có cốt truyện đan xen. Khám phá Los Santos rộng lớn và tham gia vào những phi vụ đầy kịch tính trong thế giới tội phạm đa dạng. Trải nghiệm cuộc sống của Michael, Franklin và Trevor trong những cuộc phiêu lưu không ngừng.', '2013-09-17', N'Rockstar Games', 1),
(N'Minecraft', 26.95, N'Game sandbox sáng tạo vô hạn cho phép bạn xây dựng mọi thứ từ trí tưởng tượng. Tạo ra những công trình kiến trúc tuyệt vời, khám phá hang động bí ẩn và chiến đấu với quái vật trong thế giới khối vuông vô tận. Phù hợp cho mọi lứa tuổi với khả năng sáng tạo không giới hạn.', '2011-11-18', N'Mojang Studios', 0),
(N'Red Dead Redemption 2', 59.99, N'Câu chuyện miền Tây hoang dã sâu sắc và cảm động với Arthur Morgan. Trải nghiệm cuộc sống của một tay súng trong băng đảng Van der Linde trong thời kỳ kết thúc của miền Tây hoang dã. Thế giới mở chi tiết với hệ thống honor và những lựa chọn đạo đức phức tạp.', '2018-10-26', N'Rockstar Games', 1),
(N'Assassin Creed Valhalla', 59.99, N'Cuộc phiêu lưu Viking hùng tráng trong thời đại trung cổ nước Anh. Dẫn dắt Eivor trong cuộc xâm lược và định cư, xây dựng di sản Viking của bạn. Khám phá thế giới Norse mythology với những trận chiến epic và cốt truyện về gia đình, danh dự và số phận.', '2020-11-10', N'Ubisoft', 0),
(N'Call of Duty: Modern Warfare', 59.99, N'Game bắn súng chiến thuật hiện đại với đồ họa siêu thực và gameplay căng thẳng. Tham gia vào những nhiệm vụ đặc biệt nguy hiểm và chiến đấu trực tuyến multiplayer với người chơi trên khắp thế giới. Trải nghiệm chiến tranh hiện đại với vũ khí và chiến thuật tiên tiến.', '2019-10-25', N'Infinity Ward', 0),
(N'FIFA 23', 69.99, N'Trò chơi simulation bóng đá chân thực và chi tiết nhất từ trước đến nay. Quản lý đội bóng yêu thích, thi đấu với những đội bóng hàng đầu thế giới và trải nghiệm World Cup. Với công nghệ HyperMotion2 mang lại gameplay chân thực chưa từng có.', '2022-09-30', N'EA Sports', 0),
(N'Among Us', 4.99, N'Game multiplayer xã hội đầy bí ẩn và vui nhộn trong không gian. Làm việc cùng nhau để hoàn thành nhiệm vụ trên tàu vũ trụ, nhưng hãy cẻnh chừng - trong số các thành viên có kẻ phản bội đang cố gắng sabotage và giết hại mọi người. Ai là Impostor?', '2018-06-15', N'InnerSloth', 0),
(N'Hades', 24.99, N'Game roguelike xuất sắc với cốt truyện thần thoại Hy Lạp sâu sắc. Chơi vai Zagreus - con trai của Hades, chiến đấu để thoát khỏi địa ngục trong trò chơi indie được giới phê bình đánh giá cao. Mỗi lần chết đều mang lại trải nghiệm mới với dialogue và story progression độc đáo.', '2020-09-17', N'Supergiant Games', 1),
(N'Stardew Valley', 14.99, N'Game farming simulation thư giãn và ấm áp về cuộc sống nông thôn. Xây dựng trang trại từ con số 0, nuôi động vật, trồng trọt, kết bạn với dân làng và tìm hiểu những bí mật của Pelican Town. Perfect game để thư giãn sau những ngày làm việc căng thẳng.', '2016-02-26', N'ConcernedApe', 0),
(N'Dark Souls III', 59.99, N'Game nhập vai hành động khó khăn và thử thách với combat system sâu sắc. Chiến đấu với những boss khổng lồ và khám phá thế giới đen tối gothic đầy nguy hiểm. Trò chơi đòi hỏi kỹ năng, kiên nhẫn và chiến thuật để vượt qua những thử thách khó nhằn.', '2016-04-12', N'FromSoftware', 0),
(N'Overwatch 2', 0.00, N'Game bắn súng đội nhóm hero-based miễn phí với gameplay team-oriented. Chọn từ hàng chục hero với khả năng đặc biệt và vai trò khác nhau. Chiến đấu 5v5 trong những trận đấu kịch tính với objective-based gameplay đòi hỏi teamwork và strategy.', '2022-10-04', N'Blizzard Entertainment', 1),
(N'Elden Ring', 59.99, N'Siêu phẩm nhập vai thế giới mở từ FromSoftware và George R.R. Martin. Khám phá Lands Between rộng lớn với những bí ẩn và thử thách nguy hiểm. Kết hợp combat khó khăn đặc trưng của Dark Souls với sự tự do khám phá của open world trong một thế giới fantasy tuyệt đẹp.', '2022-02-25', N'FromSoftware', 1),
(N'Fall Guys', 19.99, N'Game battle royale vui nhộn và đầy màu sắc với 60 người chơi cùng tham gia. Cạnh tranh qua các mini-game obstacle course đáng yêu để trở thành người cuối cùng đứng vững. Game phù hợp cho mọi lứa tuổi với gameplay đơn giản nhưng gây nghiện.', '2020-08-04', N'Mediatonic', 0),
(N'Valorant', 0.00, N'Game bắn súng tactical 5v5 miễn phí kết hợp FPS với abilities độc đáo. Chọn Agent với khả năng đặc biệt và phối hợp team để plant/defuse bomb. Đòi hỏi kỹ năng aim chính xác, game sense tốt và teamwork để chiến thắng trong ranked competitive.', '2020-06-02', N'Riot Games', 0),
(N'The Sims 4', 39.99, N'Game simulation cuộc sống cho phép bạn tạo ra và điều khiển những nhân vật ảo. Xây dựng ngôi nhà mơ ước, phát triển sự nghiệp, tạo gia đình và sống cuộc đời như bạn muốn. Với hàng trăm expansion packs và mods, khả năng customization gần như vô hạn.', '2014-09-02', N'Maxis', 0),
(N'Counter-Strike 2', 0.00, N'Game bắn súng tactical kinh điển được remaster với engine Source 2. Chiến đấu 5v5 competitive trong những map huyền thoại với gameplay không thay đổi theo thời gian. Đòi hỏi kỹ năng aim, game sense và teamwork để thành công trong esports scene.', '2023-09-27', N'Valve', 0),
(N'Fortnite', 0.00, N'Game battle royale miễn phí với building mechanics độc đáo. Chiến đấu với 100 người chơi khác trên map động để trở thành Victory Royale. Với việc cập nhật content thường xuyên, crossover events và creative mode, Fortnite luôn mang đến trải nghiệm mới mẻ.', '2017-07-25', N'Epic Games', 1),
(N'League of Legends', 0.00, N'Game MOBA 5v5 miễn phí phổ biến nhất thế giới với hơn 160 champions. Phối hợp với team để phá hủy Nexus của đối thủ trong những trận đấu tactical căng thẳng. Với scene esports khổng lồ và gameplay depth cao, LoL là game competitive hàng đầu.', '2009-10-27', N'Riot Games', 0);

-- Seed Game Tags (Mỗi game có 2-4 tags ngẫu nhiên)
INSERT INTO GameTags (GameId, TagId) VALUES
-- Cyberpunk 2077 (1)
(1, 1), (1, 2), (1, 3), (1, 13), (1, 15),
-- The Witcher 3 (2)
(2, 2), (2, 3), (2, 12), (2, 13), (2, 14),
-- GTA V (3)
(3, 1), (3, 2), (3, 11), (3, 13),
-- Minecraft (4)
(4, 5), (4, 9), (4, 10), (4, 11), (4, 12),
-- Red Dead Redemption 2 (5)
(5, 1), (5, 2), (5, 12), (5, 13),
-- AC Valhalla (6)
(6, 1), (6, 2), (6, 3), (6, 13),
-- COD MW (7)
(7, 1), (7, 11), (7, 12),
-- FIFA 23 (8)
(8, 6), (8, 5), (8, 11),
-- Among Us (9)
(9, 9), (9, 10), (9, 11),
-- Hades (10)
(10, 1), (10, 3), (10, 10), (10, 12),
-- Stardew Valley (11)
(11, 5), (11, 10), (11, 12),
-- Dark Souls III (12)
(12, 1), (12, 3), (12, 12), (12, 14),
-- Overwatch 2 (13)
(13, 1), (13, 11),
-- Elden Ring (14)
(14, 1), (14, 2), (14, 3), (14, 13), (14, 14),
-- Fall Guys (15)
(15, 9), (15, 11),
-- Valorant (16)
(16, 1), (16, 11),
-- The Sims 4 (17)
(17, 5), (17, 12),
-- Counter-Strike 2 (18)
(18, 1), (18, 11),
-- Fortnite (19)
(19, 1), (19, 11),
-- League of Legends (20)
(20, 4), (20, 11);

-- Seed Image URLs (5 hình cho mỗi game với chủ đề liên quan)
INSERT INTO ImageUrls (Url, GameId) VALUES
-- Cyberpunk 2077 (GameId: 1) - Sci-fi/Futuristic
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 1),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 1),
('https://images.unsplash.com/photo-1614728263952-84ea256f9679?w=800', 1),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 1),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 1),

-- The Witcher 3 (GameId: 2) - Fantasy/Medieval
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 2),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 2),
('https://images.unsplash.com/photo-1566228015668-4c45dbc4e2f5?w=800', 2),
('https://images.unsplash.com/photo-1578632749014-ca6bfbd73a8d?w=800', 2),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 2),

-- GTA V (GameId: 3) - Urban/Crime
('https://images.unsplash.com/photo-1449824913935-59a10b8d2000?w=800', 3),
('https://images.unsplash.com/photo-1480714378408-67cf0d13bc1f?w=800', 3),
('https://images.unsplash.com/photo-1514565131-fce0801e5785?w=800', 3),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 3),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 3),

-- Minecraft (GameId: 4) - Blocky/Creative
('https://images.unsplash.com/photo-1606092195730-5d7b9af1efc5?w=800', 4),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 4),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 4),
('https://images.unsplash.com/photo-1551103782-8ab07afd45c1?w=800', 4),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 4),

-- Red Dead Redemption 2 (GameId: 5) - Western/Wild West
('https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800', 5),
('https://images.unsplash.com/photo-1568605117036-5fe5e7bab0b7?w=800', 5),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 5),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 5),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 5),

-- Assassin's Creed Valhalla (GameId: 6) - Viking/Medieval
('https://images.unsplash.com/photo-1566228015668-4c45dbc4e2f5?w=800', 6),
('https://images.unsplash.com/photo-1578632749014-ca6bfbd73a8d?w=800', 6),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 6),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 6),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 6),

-- Call of Duty MW (GameId: 7) - Military/War
('https://images.unsplash.com/photo-1574717024653-61fd2cf4d44d?w=800', 7),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 7),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 7),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 7),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 7),

-- FIFA 23 (GameId: 8) - Soccer/Football
('https://images.unsplash.com/photo-1574629810360-7efbbe195018?w=800', 8),
('https://images.unsplash.com/photo-1431324155629-1a6deb1dec8d?w=800', 8),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 8),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 8),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 8),

-- Among Us (GameId: 9) - Space/Colorful
('https://images.unsplash.com/photo-1614728263952-84ea256f9679?w=800', 9),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 9),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 9),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 9),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 9),

-- Hades (GameId: 10) - Greek Mythology/Dark
('https://images.unsplash.com/photo-1578632749014-ca6bfbd73a8d?w=800', 10),
('https://images.unsplash.com/photo-1566228015668-4c45dbc4e2f5?w=800', 10),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 10),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 10),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 10),

-- Stardew Valley (GameId: 11) - Farm/Nature
('https://images.unsplash.com/photo-1500382017468-9049fed747ef?w=800', 11),
('https://images.unsplash.com/photo-1506905925346-21bda4d32df4?w=800', 11),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 11),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 11),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 11),

-- Dark Souls III (GameId: 12) - Dark Fantasy/Medieval
('https://images.unsplash.com/photo-1578632749014-ca6bfbd73a8d?w=800', 12),
('https://images.unsplash.com/photo-1566228015668-4c45dbc4e2f5?w=800', 12),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 12),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 12),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 12),

-- Overwatch 2 (GameId: 13) - Futuristic/Colorful
('https://images.unsplash.com/photo-1614728263952-84ea256f9679?w=800', 13),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 13),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 13),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 13),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 13),

-- Elden Ring (GameId: 14) - Dark Fantasy/Open World
('https://images.unsplash.com/photo-1578632749014-ca6bfbd73a8d?w=800', 14),
('https://images.unsplash.com/photo-1566228015668-4c45dbc4e2f5?w=800', 14),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 14),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 14),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 14),

-- Fall Guys (GameId: 15) - Colorful/Fun
('https://images.unsplash.com/photo-1606092195730-5d7b9af1efc5?w=800', 15),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 15),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 15),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 15),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 15),

-- Valorant (GameId: 16) - Tactical/Modern
('https://images.unsplash.com/photo-1574717024653-61fd2cf4d44d?w=800', 16),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 16),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 16),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 16),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 16),

-- The Sims 4 (GameId: 17) - Life Simulation
('https://images.unsplash.com/photo-1449824913935-59a10b8d2000?w=800', 17),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 17),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 17),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 17),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 17),

-- Counter-Strike 2 (GameId: 18) - Tactical Shooter
('https://images.unsplash.com/photo-1574717024653-61fd2cf4d44d?w=800', 18),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 18),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 18),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 18),
('https://images.unsplash.com/photo-1550745165-9bc0b252726f?w=800', 18),

-- Fortnite (GameId: 19) - Battle Royale/Colorful
('https://images.unsplash.com/photo-1606092195730-5d7b9af1efc5?w=800', 19),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 19),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 19),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 19),
('https://images.unsplash.com/photo-1578662996442-48f60103fc96?w=800', 19),

-- League of Legends (GameId: 20) - Fantasy MOBA
('https://images.unsplash.com/photo-1578632749014-ca6bfbd73a8d?w=800', 20),
('https://images.unsplash.com/photo-1566228015668-4c45dbc4e2f5?w=800', 20),
('https://images.unsplash.com/photo-1511512578047-dfb367046420?w=800', 20),
('https://images.unsplash.com/photo-1518709268805-4e9042af2ac1?w=800', 20),
('https://images.unsplash.com/photo-1542751371-adc38448a05e?w=800', 20);

-- Seed Transactions (Giao dịch xuyên suốt năm 2024)
INSERT INTO Transactions (AccountId, TotalAmount, TransactionDate, PaymentMethod, Status) VALUES
-- Q1 2024 (Tháng 1-3)
(2, 89.98, '2024-01-15 14:30:00', N'Credit Card', N'Completed'),
(3, 59.99, '2024-01-20 16:45:00', N'PayPal', N'Completed'),
(4, 126.93, '2024-02-05 10:15:00', N'Credit Card', N'Completed'),
(5, 39.99, '2024-02-14 20:30:00', N'PayPal', N'Completed'),
(6, 179.96, '2024-02-28 12:00:00', N'Credit Card', N'Completed'),
(7, 59.99, '2024-03-10 18:45:00', N'PayPal', N'Completed'),
(8, 94.98, '2024-03-22 14:20:00', N'Credit Card', N'Completed'),

-- Q2 2024 (Tháng 4-6)
(9, 149.97, '2024-04-08 11:30:00', N'Credit Card', N'Completed'),
(10, 89.98, '2024-04-25 15:15:00', N'PayPal', N'Completed'),
(11, 199.96, '2024-05-03 13:45:00', N'Credit Card', N'Completed'),
(2, 69.99, '2024-05-18 19:30:00', N'PayPal', N'Completed'),
(3, 119.98, '2024-05-30 16:00:00', N'Credit Card', N'Completed'),
(4, 84.98, '2024-06-12 21:15:00', N'PayPal', N'Completed'),
(5, 179.96, '2024-06-28 09:45:00', N'Credit Card', N'Completed'),

-- Q3 2024 (Tháng 7-9)
(6, 139.97, '2024-07-04 17:30:00', N'PayPal', N'Completed'),
(7, 99.98, '2024-07-19 14:15:00', N'Credit Card', N'Completed'),
(8, 249.95, '2024-08-02 11:00:00', N'Credit Card', N'Completed'),
(9, 74.98, '2024-08-15 18:45:00', N'PayPal', N'Completed'),
(10, 159.97, '2024-08-29 20:30:00', N'Credit Card', N'Completed'),
(11, 119.98, '2024-09-10 16:15:00', N'PayPal', N'Completed'),
(2, 189.96, '2024-09-25 13:30:00', N'Credit Card', N'Completed'),

-- Q4 2024 (Tháng 10-12)
(3, 229.95, '2024-10-05 15:45:00', N'Credit Card', N'Completed'),
(4, 94.98, '2024-10-20 12:00:00', N'PayPal', N'Completed'),
(5, 269.94, '2024-11-01 14:30:00', N'Credit Card', N'Completed'),
(6, 149.97, '2024-11-15 19:15:00', N'PayPal', N'Completed'),
(7, 199.96, '2024-11-28 16:45:00', N'Credit Card', N'Completed'),
(8, 179.96, '2024-12-05 11:30:00', N'PayPal', N'Completed'),
(9, 299.95, '2024-12-18 20:00:00', N'Credit Card', N'Completed'),
(10, 159.97, '2024-12-22 17:15:00', N'PayPal', N'Completed');

-- Seed Transaction Items (Chi tiết từng game trong giao dịch)
INSERT INTO TransactionItems (TransactionId, GameId, Quantity, UnitPrice) VALUES
-- Transaction 1 (89.98)
(1, 1, 1, 59.99), (1, 4, 1, 26.95), (1, 10, 1, 24.99), -- Cyberpunk + Minecraft + Hades
-- Transaction 2 (59.99)
(2, 2, 1, 39.99), (2, 9, 1, 4.99), (2, 11, 1, 14.99), -- Witcher + Among Us + Stardew
-- Transaction 3 (126.93)
(3, 3, 1, 29.99), (3, 5, 1, 59.99), (3, 8, 1, 69.99), -- GTA + RDR2 + FIFA
-- Transaction 4 (39.99)
(4, 2, 1, 39.99), -- Witcher 3
-- Transaction 5 (179.96)
(5, 1, 1, 59.99), (5, 5, 1, 59.99), (5, 6, 1, 59.99), -- Cyberpunk + RDR2 + AC Valhalla
-- Transaction 6 (59.99)
(6, 14, 1, 59.99), -- Elden Ring
-- Transaction 7 (94.98)
(7, 3, 1, 29.99), (7, 12, 1, 59.99), (7, 9, 1, 4.99), -- GTA + Dark Souls + Among Us

-- Transaction 8 (149.97)
(8, 1, 1, 59.99), (8, 8, 1, 69.99), (8, 15, 1, 19.99), -- Cyberpunk + FIFA + Fall Guys
-- Transaction 9 (89.98)
(9, 2, 1, 39.99), (9, 17, 1, 39.99), (9, 10, 1, 24.99), -- Witcher + Sims + Hades
-- Transaction 10 (199.96)
(10, 5, 1, 59.99), (10, 6, 1, 59.99), (10, 7, 1, 59.99), (10, 15, 1, 19.99), -- RDR2 + AC + COD + Fall Guys
-- Transaction 11 (69.99)
(11, 8, 1, 69.99), -- FIFA 23
-- Transaction 12 (119.98)
(12, 14, 1, 59.99), (12, 12, 1, 59.99), -- Elden Ring + Dark Souls
-- Transaction 13 (84.98)
(13, 3, 1, 29.99), (13, 11, 1, 14.99), (13, 2, 1, 39.99), -- GTA + Stardew + Witcher
-- Transaction 14 (179.96)
(14, 1, 1, 59.99), (14, 5, 1, 59.99), (14, 6, 1, 59.99), -- Cyberpunk + RDR2 + AC Valhalla

-- Transaction 15 (139.97)
(15, 8, 1, 69.99), (15, 14, 1, 59.99), (15, 10, 1, 24.99), -- FIFA + Elden Ring + Hades
-- Transaction 16 (99.98)
(16, 2, 1, 39.99), (16, 17, 1, 39.99), (16, 15, 1, 19.99), -- Witcher + Sims + Fall Guys
-- Transaction 17 (249.95)
(17, 1, 1, 59.99), (17, 5, 1, 59.99), (17, 6, 1, 59.99), (17, 8, 1, 69.99), -- Top games bundle
-- Transaction 18 (74.98)
(18, 3, 1, 29.99), (18, 10, 1, 24.99), (18, 15, 1, 19.99), -- GTA + Hades + Fall Guys
-- Transaction 19 (159.97)
(19, 14, 1, 59.99), (19, 12, 1, 59.99), (19, 2, 1, 39.99), -- Elden Ring + Dark Souls + Witcher
-- Transaction 20 (119.98)
(20, 1, 1, 59.99), (20, 5, 1, 59.99), -- Cyberpunk + RDR2
-- Transaction 21 (189.96)
(21, 8, 1, 69.99), (21, 6, 1, 59.99), (21, 7, 1, 59.99), -- FIFA + AC Valhalla + COD

-- Transaction 22 (229.95)
(22, 1, 1, 59.99), (22, 5, 1, 59.99), (22, 14, 1, 59.99), (22, 8, 1, 69.99), -- Premium bundle
-- Transaction 23 (94.98)
(23, 2, 1, 39.99), (23, 11, 1, 14.99), (23, 17, 1, 39.99), -- Witcher + Stardew + Sims
-- Transaction 24 (269.94)
(24, 1, 1, 59.99), (24, 5, 1, 59.99), (24, 6, 1, 59.99), (24, 8, 1, 69.99), (24, 15, 1, 19.99), -- Mega bundle
-- Transaction 25 (149.97)
(25, 14, 1, 59.99), (25, 7, 1, 59.99), (25, 3, 1, 29.99), -- Elden Ring + COD + GTA
-- Transaction 26 (199.96)
(26, 1, 1, 59.99), (26, 5, 1, 59.99), (26, 6, 1, 59.99), (26, 15, 1, 19.99), -- Holiday bundle
-- Transaction 27 (179.96)
(27, 8, 1, 69.99), (27, 14, 1, 59.99), (27, 12, 1, 59.99), -- FIFA + Elden Ring + Dark Souls
-- Transaction 28 (299.95)
(28, 1, 1, 59.99), (28, 5, 1, 59.99), (28, 6, 1, 59.99), (28, 8, 1, 69.99), (28, 17, 1, 39.99), (28, 10, 1, 24.99), -- Christmas mega bundle
-- Transaction 29 (159.97)
(29, 2, 1, 39.99), (29, 3, 1, 29.99), (29, 7, 1, 59.99), (29, 4, 1, 26.95), -- End year bundle

-- Seed Game Ownership (Các game đã mua)
INSERT INTO GamesOwned (AccountId, GameId, PurchasedAt) VALUES
-- From transactions above
(2, 1, '2024-01-15 14:30:00'), (2, 4, '2024-01-15 14:30:00'), (2, 10, '2024-01-15 14:30:00'),
(3, 2, '2024-01-20 16:45:00'), (3, 9, '2024-01-20 16:45:00'), (3, 11, '2024-01-20 16:45:00'),
(4, 3, '2024-02-05 10:15:00'), (4, 5, '2024-02-05 10:15:00'), (4, 8, '2024-02-05 10:15:00'),
(5, 2, '2024-02-14 20:30:00'),
(6, 1, '2024-02-28 12:00:00'), (6, 5, '2024-02-28 12:00:00'), (6, 6, '2024-02-28 12:00:00'),
(7, 14, '2024-03-10 18:45:00'),
(8, 3, '2024-03-22 14:20:00'), (8, 12, '2024-03-22 14:20:00'), (8, 9, '2024-03-22 14:20:00'),
-- Add more ownership records for other transactions
(9, 1, '2024-04-08 11:30:00'), (9, 8, '2024-04-08 11:30:00'), (9, 15, '2024-04-08 11:30:00'),
(10, 2, '2024-04-25 15:15:00'), (10, 17, '2024-04-25 15:15:00'), (10, 10, '2024-04-25 15:15:00'),
(11, 5, '2024-05-03 13:45:00'), (11, 6, '2024-05-03 13:45:00'), (11, 7, '2024-05-03 13:45:00'), (11, 15, '2024-05-03 13:45:00');

-- Seed Reviews (Một số đánh giá từ người dùng đã mua game)
INSERT INTO Reviews (AccountId, GameId, Rating, Comment, CreatedAt) VALUES
(2, 1, 5, N'Game tuyệt vời! Đồ họa đẹp và cốt truyện hấp dẫn.', '2024-01-20 15:30:00'),
(3, 2, 5, N'The Witcher 3 xứng đáng là game RPG hay nhất! Recommend!', '2024-01-25 18:45:00'),
(4, 5, 4, N'Red Dead Redemption 2 có thế giới rất sống động, nhưng hơi chậm.', '2024-02-10 20:15:00'),
(5, 2, 5, N'Game fantasy tuyệt vời, đã chơi hơn 100 giờ rồi!', '2024-02-20 16:30:00'),
(6, 1, 4, N'Cyberpunk đã được fix nhiều bug, giờ chơi rất mượt.', '2024-03-05 14:45:00'),
(7, 14, 5, N'Elden Ring khó nhưng rất nghiện! FromSoftware làm game quá hay.', '2024-03-15 19:20:00'),
(8, 3, 4, N'GTA V vẫn hay sau nhiều năm, online vui với bạn bè.', '2024-03-28 21:10:00'),
(9, 8, 3, N'FIFA 23 ổn nhưng không có gì mới so với năm trước.', '2024-04-12 17:30:00'),
(10, 17, 5, N'The Sims 4 rất thư giãn, tạo nhân vật và xây nhà cực kỳ vui.', '2024-04-30 13:45:00'),
(11, 5, 5, N'Red Dead 2 có câu chuyện cảm động nhất từng chơi!', '2024-05-08 22:15:00');

-- Verification Query
SELECT 
    'Accounts' as TableName, COUNT(*) as RecordCount FROM Accounts
UNION ALL
SELECT 
    'Games' as TableName, COUNT(*) as RecordCount FROM Games
UNION ALL
SELECT 
    'Tags' as TableName, COUNT(*) as RecordCount FROM Tags
UNION ALL
SELECT 
    'GameTags' as TableName, COUNT(*) as RecordCount FROM GameTags
UNION ALL
SELECT 
    'ImageUrls' as TableName, COUNT(*) as RecordCount FROM ImageUrls
UNION ALL
SELECT 
    'Transactions' as TableName, COUNT(*) as RecordCount FROM Transactions
UNION ALL
SELECT 
    'TransactionItems' as TableName, COUNT(*) as RecordCount FROM TransactionItems
UNION ALL
SELECT 
    'GamesOwned' as TableName, COUNT(*) as RecordCount FROM GamesOwned
UNION ALL
SELECT 
    'Reviews' as TableName, COUNT(*) as RecordCount FROM Reviews;

-- Revenue Analysis Query
SELECT 
    'Total Revenue 2024' as Metric, 
    FORMAT(SUM(TotalAmount), 'C', 'vi-VN') as Value
FROM Transactions 
WHERE Status = 'Completed' AND YEAR(TransactionDate) = 2024
UNION ALL
SELECT 
    'Q1 2024 Revenue' as Metric,
    FORMAT(SUM(TotalAmount), 'C', 'vi-VN') as Value
FROM Transactions 
WHERE Status = 'Completed' AND TransactionDate BETWEEN '2024-01-01' AND '2024-03-31'
UNION ALL
SELECT 
    'Q2 2024 Revenue' as Metric,
    FORMAT(SUM(TotalAmount), 'C', 'vi-VN') as Value
FROM Transactions 
WHERE Status = 'Completed' AND TransactionDate BETWEEN '2024-04-01' AND '2024-06-30'
UNION ALL
SELECT 
    'Q3 2024 Revenue' as Metric,
    FORMAT(SUM(TotalAmount), 'C', 'vi-VN') as Value
FROM Transactions 
WHERE Status = 'Completed' AND TransactionDate BETWEEN '2024-07-01' AND '2024-09-30'
UNION ALL
SELECT 
    'Q4 2024 Revenue' as Metric,
    FORMAT(SUM(TotalAmount), 'C', 'vi-VN') as Value
FROM Transactions 
WHERE Status = 'Completed' AND TransactionDate BETWEEN '2024-10-01' AND '2024-12-31';

PRINT 'Seed data completed successfully!';
PRINT '- 11 Accounts (1 Admin + 10 Users)';
PRINT '- 20 Games with detailed information';
PRINT '- 15 Tags';
PRINT '- 100 Image URLs (5 per game)';
PRINT '- 29 Transactions throughout 2024';
PRINT '- Transaction Items and Game Ownership';
PRINT '- 10 User Reviews';
PRINT '- Game-Tag relationships established'; 