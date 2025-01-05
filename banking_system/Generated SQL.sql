if exists (select 1
            from  sysindexes
           where  id    = object_id('ACCOUNT')
            and   name  = 'OWNS_FK'
            and   indid > 0
            and   indid < 255)
   drop index ACCOUNT.OWNS_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('ACCOUNT')
            and   type = 'U')
   drop table ACCOUNT
go

if exists (select 1
            from  sysobjects
           where  id = object_id('BANK')
            and   type = 'U')
   drop table BANK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('BORROWS')
            and   name  = 'BORROWS2_FK'
            and   indid > 0
            and   indid < 255)
   drop index BORROWS.BORROWS2_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('BORROWS')
            and   name  = 'BORROWS_FK'
            and   indid > 0
            and   indid < 255)
   drop index BORROWS.BORROWS_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('BORROWS')
            and   type = 'U')
   drop table BORROWS
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('BRANCH')
            and   name  = 'OPERATES_FK'
            and   indid > 0
            and   indid < 255)
   drop index BRANCH.OPERATES_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('BRANCH')
            and   type = 'U')
   drop table BRANCH
go

if exists (select 1
            from  sysobjects
           where  id = object_id('CUSTOMER')
            and   type = 'U')
   drop table CUSTOMER
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('HAS')
            and   name  = 'HAS2_FK'
            and   indid > 0
            and   indid < 255)
   drop index HAS.HAS2_FK
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('HAS')
            and   name  = 'HAS_FK'
            and   indid > 0
            and   indid < 255)
   drop index HAS.HAS_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('HAS')
            and   type = 'U')
   drop table HAS
go

if exists (select 1
            from  sysindexes
           where  id    = object_id('LOAN')
            and   name  = 'OFFERS_FK'
            and   indid > 0
            and   indid < 255)
   drop index LOAN.OFFERS_FK
go

if exists (select 1
            from  sysobjects
           where  id = object_id('LOAN')
            and   type = 'U')
   drop table LOAN
go

/==============================================================/
/* Table: ACCOUNT                                               */
/==============================================================/
create table ACCOUNT (
   ACCOUNTNUMBER        int                  not null,
   SSN                  varchar(14)          not null,
   TYPE                 varchar(30)          not null,
   BALANCE              numeric              not null,
   constraint PK_ACCOUNT primary key (ACCOUNTNUMBER)
)
go

/==============================================================/
/* Index: OWNS_FK                                               */
/==============================================================/




create nonclustered index OWNS_FK on ACCOUNT (SSN ASC)
go

/==============================================================/
/* Table: BANK                                                  */
/==============================================================/
create table BANK (
   BANKCODE             int                  not null,
   NAME                 varchar(50)          not null,
   ADDRESS              varchar(200)         not null,
   constraint PK_BANK primary key (BANKCODE)
)
go

/==============================================================/
/* Table: BORROWS                                               */
/==============================================================/
create table BORROWS (
   BANKCODE             int                  not null,
   BRANCHNUMBER         int                  not null,
   LOANNUMBER           int                  not null,
   SSN                  varchar(14)          not null,
   AMOUNT               numeric              not null,
   START_DATE           datetime             not null,
   constraint PK_BORROWS primary key (BANKCODE, BRANCHNUMBER, LOANNUMBER, SSN)
)
go

/==============================================================/
/* Index: BORROWS_FK                                            */
/==============================================================/




create nonclustered index BORROWS_FK on BORROWS (BANKCODE ASC,
  BRANCHNUMBER ASC,
  LOANNUMBER ASC)
go

/==============================================================/
/* Index: BORROWS2_FK                                           */
/==============================================================/




create nonclustered index BORROWS2_FK on BORROWS (SSN ASC)
go

/==============================================================/
/* Table: BRANCH                                                */
/==============================================================/
create table BRANCH (
   BANKCODE             int                  not null,
   BRANCHNUMBER         int                  not null,
   ADDRESS              varchar(200)         not null,
   constraint PK_BRANCH primary key (BANKCODE, BRANCHNUMBER)
)
go

/==============================================================/
/* Index: OPERATES_FK                                           */
/==============================================================/




create nonclustered index OPERATES_FK on BRANCH (BANKCODE ASC)
go

/==============================================================/
/* Table: CUSTOMER                                              */
/==============================================================/
create table CUSTOMER (
   SSN                  varchar(14)          not null,
   NAME                 varchar(50)          not null,
   PHONE                varchar(11)          null,
   ADDRESS              varchar(200)         not null,
   constraint PK_CUSTOMER primary key (SSN)
)
go

/==============================================================/
/* Table: HAS                                                   */
/==============================================================/
create table HAS (
   BANKCODE             int                  not null,
   BRANCHNUMBER         int                  not null,
   SSN                  varchar(14)          not null,
   constraint PK_HAS primary key (BANKCODE, BRANCHNUMBER, SSN)
)
go

/==============================================================/
/* Index: HAS_FK                                                */
/==============================================================/




create nonclustered index HAS_FK on HAS (BANKCODE ASC,
  BRANCHNUMBER ASC)
go

/==============================================================/
/* Index: HAS2_FK                                               */
/==============================================================/




create nonclustered index HAS2_FK on HAS (SSN ASC)
go

/==============================================================/
/* Table: LOAN                                                  */
/==============================================================/
create table LOAN (
   BANKCODE             int                  not null,
   BRANCHNUMBER         int                  not null,
   LOANNUMBER           int                  not null,
   TYPE                 varchar(30)          not null,
   constraint PK_LOAN primary key (BANKCODE, BRANCHNUMBER, LOANNUMBER)
)
go

/==============================================================/
/* Index: OFFERS_FK                                             */
/==============================================================/




create nonclustered index OFFERS_FK on LOAN (BANKCODE ASC,
  BRANCHNUMBER ASC)
go
alter table ACCOUNT
   add constraint FK_ACCOUNT_OWNS_CUSTOMER foreign key (SSN)
      references CUSTOMER (SSN)
go
alter table BORROWS
   add constraint FK_BORROWS_BORROWS_LOAN foreign key (BANKCODE, BRANCHNUMBER, LOANNUMBER)
      references LOAN (BANKCODE, BRANCHNUMBER, LOANNUMBER)
go
alter table BORROWS
   add constraint FK_BORROWS_BORROWS2_CUSTOMER foreign key (SSN)
      references CUSTOMER (SSN)
go
alter table BRANCH
   add constraint FK_BRANCH_OPERATES_BANK foreign key (BANKCODE)
      references BANK (BANKCODE)
go
alter table HAS
   add constraint FK_HAS_HAS_BRANCH foreign key (BANKCODE, BRANCHNUMBER)
      references BRANCH (BANKCODE, BRANCHNUMBER)
go
alter table HAS
   add constraint FK_HAS_HAS2_CUSTOMER foreign key (SSN)
      references CUSTOMER (SSN)
go
alter table LOAN
   add constraint FK_LOAN_OFFERS_BRANCH foreign key (BANKCODE, BRANCHNUMBER)
      references BRANCH (BANKCODE, BRANCHNUMBER)
go