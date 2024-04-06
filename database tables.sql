CREATE TABLE roles (
    role_id UUID NOT NULL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
	UNIQUE(name)
);

create table users (
	user_id UUID NOT NULL PRIMARY KEY,
	username VARCHAR(30) NOT NULL,
	password_hash VARCHAR(64) ,
	role_id UUID REFERENCES roles(role_id) NOT NULL,
	UNIQUE(username)
);

CREATE TABLE categories (
    category_id UUID NOT NULL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
	UNIQUE(name)
);

create table products (
	product_id UUID NOT NULL PRIMARY KEY,
	product_name VARCHAR(50) NOT NULL,
	bar_code VARCHAR(50) NOT NULL,
    price INT,
	quantity INT NOT NULL,
	status VARCHAR(20) NOT NULL,
	category_id UUID REFERENCES categories(category_id),
	UNIQUE(product_name),
	UNIQUE(bar_code)
);
