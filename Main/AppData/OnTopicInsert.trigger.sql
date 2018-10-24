DROP TRIGGER OnTopicInsert;
CREATE TRIGGER IF NOT EXISTS OnTopicInsert
AFTER INSERT ON Topic 
WHEN NEW.Status = 1
BEGIN

    -- Update / Insert category summary.
    UPDATE CategorySummary
    SET TotalPost = TotalPost + 1,
        LastTopicId = NEW.Id,
        LastTopicTitle = NEW.Title,
        LastTopicCreatedTime = NEW.CreatedTime
    WHERE CategoryId=NEW.CategoryId;
        
    INSERT OR IGNORE INTO CategorySummary(CategoryId, TotalPost, TotalFollower, LastTopicId, LastTopicTitle, LastTopicCreatedTime)
    VALUES(NEW.CategoryId, 1, 0, NEW.Id, NEW.Title, NEW.CreatedTime);
END

