DROP TRIGGER IF EXISTS OnReplyDelete;
CREATE TRIGGER IF NOT EXISTS OnReplyDelete
AFTER UPDATE ON Reply 
WHEN NEW.Status <> OLD.Status AND NEW.Status = 0
BEGIN

    -- Decrease total reply by 1
    UPDATE TopicSummary
    SET TotalReply = TotalReply - 1
    WHERE TopicId=NEW.TopicId;
        
END

