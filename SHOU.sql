create table Users
(
	Id varchar(36) not null,
	Name nvarchar(250) not null,
	Password varchar(250) not null,
	Email varchar(250),
	Phone varchar(250),
	Address nvarchar(250),
	Avatar nvarchar(1000),
	Birthday datetime,
	Gender bit,

	constraint id_user primary key (Id) 
)

create table Images(
	Id varchar(36) not null,
	IdUser varchar(36) not null,
	ImageUrl nvarchar(250),

	constraint id_image primary key (Id)
)

create table Posts(
	Id varchar(36) not null,
	IdUser varchar(36) not null,
	IdImage varchar(36),
	Content nvarchar(max),
	Video nvarchar(250),
	Create_at datetime,

	constraint id_post primary key (Id)
)

create table Likes(
	Id varchar(36) not null,
	IdUser varchar(36) not null,
	IdPost varchar(36) not null,

	constraint id_like primary key (Id)
)

create table Comments(
	Id varchar(36) not null,
	IdUser varchar(36) not null,
	IdPost varchar(36) not null,
	Comment nvarchar(max),
	IdParent varchar(36),
	AtTime datetime,

	constraint id_comment primary key (Id)
)
create table Notification(
	Id varchar(36) not null,
	IdPost varchar(36) not null,
	IdUser varchar(36) not null,
	Type nvarchar(250),
	AtTime datetime,

	constraint id_noti primary key (Id)
)