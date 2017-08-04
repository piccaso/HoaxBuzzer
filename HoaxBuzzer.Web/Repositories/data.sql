SET SEARCH_PATH TO db, public;

-- insert articles
DO $$
DECLARE
  image_id INTEGER;
  group_id INTEGER;
BEGIN

  -- insert a new image and store id in `image_id`
  INSERT INTO image (data, "contentType") VALUES (
    decode('R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==','base64'),
    'image/gif' -- its a small transparent gif...
  ) RETURNING id INTO image_id;

  -- create a new articleGroup and store id in `group_id`
  INSERT INTO "articleGroup" (id) VALUES (DEFAULT) RETURNING id INTO group_id;

  -- insert some articles
  INSERT INTO article ("fkGroup", "sourceUrl", "articleIsTrue", "fkScreenshot", heading, summary, "metricsAlteration", "metricsTrue", "fbShares", "fbLikes", "fbComments", "twRetweets", "twLikes") VALUES
    (group_id, 'http://some/url', TRUE  , image_id, 'heading' || group_id, 'summary' || group_id || '-1',  0, TRUE  , 10, 10, 10, 10, 10),
    (group_id, 'http://some/url', FALSE , image_id, 'heading' || group_id, 'summary' || group_id || '-2', +1, FALSE , 15, 15, 15, 15, 15),
    (group_id, 'http://some/url', FALSE , image_id, 'heading' || group_id, 'summary' || group_id || '-3', -1, FALSE ,  5,  5,  5,  5,  5);


END
$$;

-- insert articles with image from url
DO $$
DECLARE
  image_id INTEGER;
  group_id INTEGER;
BEGIN

  -- insert a new image and store id in `image_id`
  INSERT INTO image (data, "contentType") VALUES (
    http_get('http://lorempixel.com/400/200/'),
    'image/gif' -- its a small transparent gif...
  ) RETURNING id INTO image_id;

  -- create a new articleGroup and store id in `group_id`
  INSERT INTO "articleGroup" (id) VALUES (DEFAULT) RETURNING id INTO group_id;

  -- insert some articles
  INSERT INTO article ("fkGroup", "sourceUrl", "articleIsTrue", "fkScreenshot", heading, summary, "metricsAlteration", "metricsTrue", "fbShares", "fbLikes", "fbComments", "twRetweets", "twLikes") VALUES
    (group_id, 'http://some/url', TRUE  , image_id, 'true heading' || group_id, 'true summary' || group_id || '-1',  0, TRUE  , 10, 10, 10, 10, 10),
    (group_id, 'http://some/url', FALSE , image_id, 'fake heading' || group_id, 'fake summary' || group_id || '-2', +1, FALSE , 15, 15, 15, 15, 15),
    (group_id, 'http://some/url', FALSE , image_id, 'fake heading' || group_id, 'fake summary' || group_id || '-3', -1, FALSE ,  5,  5,  5,  5,  5);


END
$$;

-- create votes
DO $$
DECLARE
  vote_id INTEGER;
  article_id INTEGER;
  vote_value BOOLEAN;
BEGIN

  -- random vote (true/false)
  SELECT INTO vote_value random() > 0.5;

  -- get the next article and store only the id
  SELECT INTO article_id (SELECT id FROM getNextArticle());

  --
  INSERT INTO vote (value, "fkArticle") VALUES (vote_value, article_id) RETURNING vote.id INTO vote_id;

  RAISE NOTICE 'vote_id=%, vote_value=%, article_id=%, group=%', vote_id, vote_value, article_id, (SELECT "fkGroup" FROM article WHERE id=article_id);

END
$$;



