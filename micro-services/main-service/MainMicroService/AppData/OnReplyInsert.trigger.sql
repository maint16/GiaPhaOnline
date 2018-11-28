DROP TRIGGER OnReplyInsert;
CREATE TRIGGER IF NOT EXISTS OnReplyInsert
AFTER INSERT ON Reply 
WHEN NEW.Status = 1
BEGIN

    -- Increase total reply by 1
    UPDATE TopicSummary
    SET TotalReply = TotalReply + 1
    WHERE TopicId=NEW.TopicId;
    
    INSERT OR IGNORE INTO TopicSummary(TopicId, TotalFollower, TotalReply)
    VALUES(NEW.TopicId, 0, 0);
    
END

