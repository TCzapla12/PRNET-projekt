-- init.sql

CREATE TABLE IF NOT EXISTS users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email TEXT UNIQUE NOT NULL, -- Walidacja na front-end/back-end
    username TEXT UNIQUE NOT NULL,
    password_hash TEXT NOT NULL, -- Hash PKCS2
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
	-- Zakladamy, ze nr tel i adres moze byc dodany pozniej. Konto moze istniec bez tego, ale jest nieaktywne
    phone TEXT UNIQUE,
    pesel TEXT UNIQUE,
    is_activated BOOLEAN DEFAULT FALSE NOT NULL,
    is_verified BOOLEAN DEFAULT FALSE NOT NULL,
    is_banned BOOLEAN DEFAULT FALSE NOT NULL,
    is_admin BOOLEAN DEFAULT FALSE NOT NULL,
    avatar_url TEXT,
    document_url TEXT[],
    created_date BIGINT NOT NULL DEFAULT EXTRACT(EPOCH FROM NOW())
);


CREATE TABLE IF NOT EXISTS addresses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
	street TEXT NOT NULL,
    house_number TEXT NOT NULL,
	apartment_number TEXT,
    city TEXT NOT NULL,
    post_code TEXT NOT NULL,
    description TEXT,
    is_primary BOOLEAN,
	owner_id UUID NOT NULL REFERENCES users(id) ON DELETE NO ACTION
);

CREATE TABLE IF NOT EXISTS animals (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    name VARCHAR(255),
    owner_id UUID NOT NULL REFERENCES users(id) ON DELETE NO ACTION,
    type VARCHAR(50) CHECK (type IN ('dog', 'cat', 'other')),
    photos TEXT[], -- URL do zdjec zwierzaka
    description TEXT
);

CREATE TABLE IF NOT EXISTS announcements (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    -- Ogloszenia zachowywane nawet przy usunieciu usera dla archiwizacji
    author_id UUID NOT NULL REFERENCES users(id) ON DELETE NO ACTION,
    keeper_id UUID REFERENCES users(id) ON DELETE NO ACTION,
    animal_id UUID NOT NULL REFERENCES animals(id) ON DELETE NO ACTION,
    keeper_profit INT NOT NULL CHECK (keeper_profit > 0), -- Zysk musi byc wiekszy niz 0. Mozna tez to po prostu sprawdzac na front/back
    is_negotiable BOOLEAN NOT NULL DEFAULT FALSE,
    
    description TEXT,
	-- Wszystkie timestampy jako UNIX 
    start_term BIGINT NOT NULL,
    end_term BIGINT NOT NULL,
    created_date BIGINT NOT NULL DEFAULT EXTRACT(EPOCH FROM NOW()),
    started_date BIGINT, -- Timestamp kiedy rzeczywiscie zwierze zostanie oddane
    finished_date BIGINT, -- Timestamp kiedy rzeczywiscie zwierze zostanie zwrocone
    
    status TEXT NOT NULL CHECK (status IN ('created', 'pending', 'accepted', 'ongoing', 'finished', 'canceled')),

    address_id UUID REFERENCES addresses(id) ON DELETE NO ACTION  -- 
);

CREATE TABLE IF NOT EXISTS opinions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    keeper_id UUID NOT NULL REFERENCES users(id) ON DELETE NO ACTION,
    author_id UUID NOT NULL REFERENCES users(id) ON DELETE NO ACTION,
    rating INT CHECK (rating >= 0 AND rating <= 10),
    description TEXT,
    announcement_id UUID REFERENCES announcements(id) ON DELETE NO ACTION UNIQUE,
    date BIGINT DEFAULT EXTRACT(EPOCH FROM NOW())
);


