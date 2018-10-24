SELECT Category.Id AS CategoryId,
       (SELECT COUNT(Id) FROM Topic WHERE Topic.CategoryId = Category.Id) AS TotalPost,
       (SELECT COUNT(FollowerId) FROM FollowCategory WHERE FollowCategory.CategoryId = Category.Id) AS TotalFollower,
       (SELECT Topic.Id FROM Topic WHERE Topic.CategoryId = Category.Id ORDER BY Topic.CreatedTime DESC) AS LastTopicId,
       (SELECT Topic.Title FROM Topic WHERE Topic.CategoryId = Category.Id ORDER BY Topic.CreatedTime DESC) AS LastTopicTilte,
       (SELECT Topic.CreatedTime FROM Topic WHERE Topic.CategoryId = Category.Id ORDER BY Topic.CreatedTime DESC) AS LastTopicCreatedTime
FROM Category 
JOIN Topic ON Category.Id = Topic.CategoryId
JOIN FollowCategory ON FollowCategory.CategoryId = Category.Id
GROUP BY Category.Id, TotalPost, TotalFollower, LastTopicId, LastTopicTilte, LastTopicCreatedTime