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
    (group_id, 'https://www.welt.de/vermischtes/article1480019/Unglaublich-aber-wahr-Der-schwangere-indische-Mann.html', TRUE  , image_id, 'Der schwangere indische Mann' || group_id, 'Der 36-jährige Sanju Bhagat lässt sich mit einem ungewöhnlich dicken Bauch in ein indisches Krankenhaus einliefern, doch als die Ärzte ihn aufschneiden, überkommt sie der blanke Horror: In seinem Körper befindet sich ein zweiter Mensch.' || group_id || '-1',  0, TRUE  , 0, 0, 0, 0, 0),
    (group_id, 'http://www.politico.eu/article/romanian-election-pits-three-men-with-same-name-against-each-other-vasile-cepoi-draguseni', TRUE , image_id, 'Romanian election pits three men with same name against each other' || group_id, 'Romanian election pits three men with same name against each other' || group_id || '-2', +1, FALSE , 15, 15, 15, 15, 15),
    (group_id, 'http://www.bbc.com/future/story/20170315-the-invention-of-heterosexuality', TRUE , image_id, 'The invention of heterosexuality' || group_id, 'One hundred years ago, people had a very different idea of what it means to be heterosexual. Dorland’s Medical Dictionary defined heterosexuality as an abnormal or perverted appetite toward the opposite sex.' || group_id || '-3', -1, FALSE ,  88601,  0,  0,  0,  0),
    (group_id, 'http://www.villaitaliankitchen.com/blog/post/introducing-the-pizzakini-the-worlds-most-mouthwatering-bikini', TRUE , image_id, '$10,000 Handmade Pizza Bikini' || group_id, 'Villa Italian Kitchen Launches $10,000 Handmade Bikini Made Entirely of Pizza in Celebration of National Bikini Day.' || group_id || '-3', -1, FALSE ,  88601,  0,  0,  0,  0),
    (group_id, 'http://derstandard.at/2000060633382/Volvo-Kaengurus-lassen-selbstfahrende-Autos-verzweifeln', TRUE , image_id, 'Volvo: Kängurus lassen selbstfahrende Autos verzweifeln' || group_id, 'Volvos selbstfahrende Autos haben ein Problem mit Kängurus. Die Software kann laut Kevin McCann, dem Australien-Chef der schwedischen Automarke die einzigartige Fortbewegungsart der Tiere nicht analysieren und dementsprechend reagieren. Bei Rentieren und Elchen hingegen würde das selbstfahrende Auto bereits adäquat reagieren.' || group_id || '-3', -1, FALSE ,  88601,  0,  0,  0,  0),
    (group_id, 'http://indianexpress.com/article/india/india-news-india/assam-unique-frog-wedding-organised-to-appease-rain-god-4400839', TRUE , image_id, 'Frog Wedding organised to appease rain god' || group_id, 'According to mythological belief, holding a frog marriage ensures ample rainfall. As part of the wedding, a wild female frog is married off to a wooden figurine, representative of the Hindu god of rain, Indra.' || group_id || '-3', -1, FALSE ,  88601,  0,  0,  0,  0),
    (group_id, 'https://www.wired.com/story/lone-star-tick-that-gives-people-meat-allergies-may-be-spreading', TRUE , image_id, 'Tick Gives People Meat Allergies' || group_id, ' In the last decade and a half, thousands of previously protein-loving Americans have developed a dangerous allergy to meat. And they all have one thing in common: the lone star tick.' || group_id || '-3', -1, FALSE ,  88601,  0,  0,  0,  0),
    (group_id, 'http://www.abc.net.au/news/2016-12-12/no-one-home-after-seven-hour-police-standoff-in-melton/8114164', TRUE , image_id, 'Polizei belagert 7 Stunden lang leeres Haus' || group_id, 'Police have been left red-faced after embarking on a seven-hour standoff outside a Melton house west of Melbourne, only to discover no-one was inside.' || group_id || '-3', -1, FALSE ,  88601,  0,  0,  0,  0),
    (group_id, 'http://kxan.com/2017/04/19/drug-sniffing-lizard-joins-arizona-police-force', FALSE , image_id, 'Drug-sniffing lizard joins Arizona police force' || group_id, 'One hard working reptile in Arizona is making a name for himself. This frenzy all began when the Avondale Police Department posted these photos on Facebook of the chief swearing in the department’s pet bearded dragon.' || group_id || '-3', -1, FALSE ,  88601,  0,  0,  0,  0),
    (group_id, 'http://www.snopes.com/pregnant/youngestmother.asp', TRUE , image_id, 'World’s Youngest Mother — 5 Years Old' || group_id, 'The youngest mother on record was a five-year-old Peruvian girl.' || group_id || '-3', -1, FALSE ,  88601,  0,  0,  0,  0),
    (group_id, 'http://science.orf.at/stories/2852839', TRUE , image_id, 'Gottesanbeterinnen fressen auch Vögel' || group_id, 'Mit ihren kräftigen Fangbeinen können Gottesanbeterinnen neben Insekten und Spinnen auch kleine Wirbeltiere erbeuten, darunter Frösche, Eidechsen, Salamander und Schlangen. Nun berichten Zoologen, dass die Raubinsekten auch kleine Vögel jagen und fressen.' || group_id || '-3', -1, FALSE ,  0,  0,  0,  0,  0);


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
    http_get('http://lorempixel.com/400/200/'), 'image/jpeg'
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



