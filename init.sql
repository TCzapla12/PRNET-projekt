-- init.sql

CREATE TABLE IF NOT EXISTS users (
    email TEXT PRIMARY KEY, -- Walidacja na front-end/back-end
    username TEXT UNIQUE NOT NULL,
    password_hash TEXT NOT NULL, -- Hash PKCS2
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
	-- Zakladamy, ze nr tel i adres moze byc dodany pozniej. Konto moze istniec bez tego, ale jest nieaktywne
    phone TEXT,
    pesel TEXT UNIQUE,
    is_activated BOOLEAN DEFAULT FALSE NOT NULL,
    is_verified BOOLEAN DEFAULT FALSE NOT NULL,
    is_banned BOOLEAN DEFAULT FALSE NOT NULL,
    is_admin BOOLEAN DEFAULT FALSE NOT NULL,
    avatar_url TEXT,
    document_url TEXT,
    created_date BIGINT NOT NULL DEFAULT EXTRACT(EPOCH FROM NOW())
);


CREATE TABLE IF NOT EXISTS addresses (
    id UUID PRIMARY KEY,
	is_primary BOOLEAN,
	address_street TEXT NOT NULL,
    address_house_number TEXT,
    address_city TEXT NOT NULL,
    address_post_code TEXT NOT NULL,
	owner_email TEXT NOT NULL REFERENCES users(email) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS announcements (
    id UUID PRIMARY KEY,
    author_email TEXT NOT NULL REFERENCES users(email) ON DELETE NO ACTION,  -- Ogloszenia zachowywane nawet przy usunieciu usera dla archiwizacji
    keeper_email TEXT NOT NULL REFERENCES users(email) ON DELETE NO ACTION,
    keeper_profit INT NOT NULL CHECK (keeper_profit > 0), -- Zysk musi byc wiekszy niz 0. Mozna tez to po prostu sprawdzac na front/back
    is_negotiable BOOLEAN NOT NULL DEFAULT FALSE,
    long_term BOOLEAN NOT NULL DEFAULT FALSE,
    
    description TEXT,
	-- Wszystkie timestampy jako UNIX 
    start_term BIGINT NOT NULL,
    end_term BIGINT NOT NULL,
    created_date BIGINT NOT NULL DEFAULT EXTRACT(EPOCH FROM NOW()),
    started_date BIGINT, -- Timestamp kiedy rzeczywiscie zwierze zostanie oddane
    finished_date BIGINT, -- Timestamp kiedy rzeczywiscie zwierze zostanie odebrane
    
    status TEXT NOT NULL CHECK (status IN ('created', 'pending', 'accepted', 'ongoing', 'finished', 'canceled')),

    address UUID REFERENCES addresses(id) ON DELETE SET NULL  -- 
    -- CONSTRAINT chk_date_range CHECK (start_term < end_term) -- Ensure logical date ranges
);

CREATE TABLE IF NOT EXISTS opinions (
    id UUID PRIMARY KEY,
    keeperEmail TEXT NOT NULL REFERENCES users(email) ON DELETE NO ACTION,
    authorEmail TEXT NOT NULL REFERENCES users(email) ON DELETE NO ACTION,
    score INT CHECK (score >= 0 AND score <= 10),
    description TEXT,
    announcementId UUID REFERENCES announcements(id) ON DELETE NO ACTION UNIQUE,
    date BIGINT DEFAULT EXTRACT(EPOCH FROM NOW())
);

CREATE TABLE IF NOT EXISTS animals (
    id UUID PRIMARY KEY,
    name VARCHAR(255),
    ownerEmail TEXT NOT NULL REFERENCES users(email) ON DELETE CASCADE,
    type VARCHAR(50) CHECK (type IN ('dog', 'cat', 'other')),
    photos TEXT[], -- URL do zdjec zwierzaka
    description TEXT
);
