SELECT Topic.Id AS TopicId,
       (SELECT COUNT(FollowerId) FROM FollowTopic WHERE Topic.Id = FollowTopic.TopicId) AS TotalFollower,
       (SELECT COUNT(Id) FROM Reply WHERE Reply.TopicId = Topic.Id) AS TotalReply
FROM Topic 
LEFT JOIN Reply ON Topic.Id = Reply.TopicId
LEFT JOIN FollowTopic ON FollowTopic.TopicId = Topic.Id
GROUP BY Topic.Id, TotalFollower, TotalReply