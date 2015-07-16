drop database if exists world_content;

-- create a database called world_content
create database world_content;

-- switch to the world_content database
use world_content;

--
-- Table structure for table `abilities`
--

DROP TABLE IF EXISTS `abilities`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `abilities` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `icon` varchar(256) DEFAULT NULL,
  `abilityType` varchar(64) DEFAULT NULL,
  `skill` int(11) DEFAULT NULL,
  `passive` tinyint(1) DEFAULT NULL,
  `activationCost` int(11) DEFAULT NULL,
  `activationCostType` varchar(32) DEFAULT NULL,
  `activationLength` float DEFAULT NULL,
  `activationAnimation` varchar(32) DEFAULT NULL,
  `activationParticles` varchar(32) DEFAULT NULL,
  `casterEffectRequired` int(11) DEFAULT NULL,
  `casterEffectConsumed` tinyint(1) DEFAULT NULL,
  `targetEffectRequired` int(11) DEFAULT NULL,
  `targetEffectConsumed` tinyint(1) DEFAULT NULL,
  `weaponRequired` varchar(32) DEFAULT NULL,
  `reagentRequired` int(11) NOT NULL DEFAULT '-1',
  `reagentConsumed` tinyint(1) DEFAULT NULL,
  `maxRange` int(11) DEFAULT NULL,
  `minRange` int(11) DEFAULT NULL,
  `aoeRadius` int(11) NOT NULL DEFAULT '0',
  `targetType` varchar(32) DEFAULT NULL,
  `targetState` int(11) DEFAULT NULL,
  `speciesTargetReq` varchar(32) DEFAULT NULL,
  `specificTargetReq` varchar(64) DEFAULT NULL,
  `globalCooldown` tinyint(1) DEFAULT NULL,
  `cooldown1Type` varchar(32) DEFAULT NULL,
  `cooldown1Duration` float DEFAULT NULL,
  `weaponCooldown` tinyint(1) DEFAULT NULL,
  `activationEffect1` int(1) DEFAULT NULL,
  `activationTarget1` varchar(32) DEFAULT NULL,
  `activationEffect2` int(11) DEFAULT NULL,
  `activationTarget2` varchar(32) DEFAULT NULL,
  `activationEffect3` int(11) DEFAULT NULL,
  `activationTarget3` varchar(32) DEFAULT NULL,
  `coordEffect1event` varchar(32) DEFAULT NULL,
  `coordEffect1` varchar(64) DEFAULT NULL,
  `coordEffect2event` varchar(32) DEFAULT NULL,
  `coordEffect2` varchar(64) DEFAULT NULL,
  `tooltip` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `abilities`
--

LOCK TABLES `abilities` WRITE;
/*!40000 ALTER TABLE `abilities` DISABLE KEYS */;
INSERT INTO `abilities` VALUES (1,'player attack ability','','CombatMeleeAbility',-1,0,0,'mana',0,'','',0,0,0,0,'None',-1,0,4,0,0,'Enemy',1,'Any','',0,'',0,0,1,'target',0,'target',0,'target','activating','Attack Effect','','',''),(4,'Fireball','','MagicalAttackAbility',2,0,0,'mana',0,'','',0,0,0,0,'None',-1,0,20,0,1,'Enemy',1,'Any','',1,'',0,0,4,'target',0,'target',0,'target','completed','Fireball','activating','~ none ~','');
/*!40000 ALTER TABLE `abilities` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `achievement_categories`
--

DROP TABLE IF EXISTS `achievement_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `achievement_categories` (
  `name` varchar(64) NOT NULL,
  PRIMARY KEY (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `achievement_categories`
--

LOCK TABLES `achievement_categories` WRITE;
/*!40000 ALTER TABLE `achievement_categories` DISABLE KEYS */;
INSERT INTO `achievement_categories` VALUES ('General'),('Minigames'),('Tasks'),('World Events');
/*!40000 ALTER TABLE `achievement_categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `achievement_criteria`
--

DROP TABLE IF EXISTS `achievement_criteria`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `achievement_criteria` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `achievementID` int(11) NOT NULL,
  `event` varchar(32) NOT NULL,
  `eventCount` int(11) DEFAULT NULL,
  `resetEvent1` varchar(32) DEFAULT NULL,
  `resetEvent2` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `achievement_criteria`
--

LOCK TABLES `achievement_criteria` WRITE;
/*!40000 ALTER TABLE `achievement_criteria` DISABLE KEYS */;
/*!40000 ALTER TABLE `achievement_criteria` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `achievement_subcategories`
--

DROP TABLE IF EXISTS `achievement_subcategories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `achievement_subcategories` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `category` varchar(64) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `category` (`category`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `achievement_subcategories`
--

LOCK TABLES `achievement_subcategories` WRITE;
/*!40000 ALTER TABLE `achievement_subcategories` DISABLE KEYS */;
INSERT INTO `achievement_subcategories` VALUES (1,'Muncher','Minigames'),(2,'Bomber','Minigames'),(3,'CTF','Minigames'),(4,'Summer Festival','World Events'),(5,'Halloween','World Events');
/*!40000 ALTER TABLE `achievement_subcategories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `achievements`
--

DROP TABLE IF EXISTS `achievements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `achievements` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `category` varchar(32) NOT NULL,
  `subcategory` varchar(32) DEFAULT NULL,
  `points` int(11) NOT NULL,
  `text` text,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `achievements`
--

LOCK TABLES `achievements` WRITE;
/*!40000 ALTER TABLE `achievements` DISABLE KEYS */;
/*!40000 ALTER TABLE `achievements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `alter_skill_current_effects`
--

DROP TABLE IF EXISTS `alter_skill_current_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `alter_skill_current_effects` (
  `id` int(11) NOT NULL,
  `skillToAlter` int(11) NOT NULL,
  `alterValue` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `alter_skill_current_effects`
--

LOCK TABLES `alter_skill_current_effects` WRITE;
/*!40000 ALTER TABLE `alter_skill_current_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `alter_skill_current_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arena_categories`
--

DROP TABLE IF EXISTS `arena_categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arena_categories` (
  `id` int(11) NOT NULL,
  `skin1` varchar(64) NOT NULL,
  `skin2` varchar(64) DEFAULT NULL,
  `skin3` varchar(64) DEFAULT NULL,
  `skin4` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arena_categories`
--

LOCK TABLES `arena_categories` WRITE;
/*!40000 ALTER TABLE `arena_categories` DISABLE KEYS */;
/*!40000 ALTER TABLE `arena_categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arena_teams`
--

DROP TABLE IF EXISTS `arena_teams`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arena_teams` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `arenaID` int(11) NOT NULL,
  `name` varchar(32) NOT NULL,
  `size` int(11) NOT NULL,
  `race` varchar(32) DEFAULT NULL,
  `goal` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arena_teams`
--

LOCK TABLES `arena_teams` WRITE;
/*!40000 ALTER TABLE `arena_teams` DISABLE KEYS */;
/*!40000 ALTER TABLE `arena_teams` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `arena_templates`
--

DROP TABLE IF EXISTS `arena_templates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `arena_templates` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `arenaType` int(11) NOT NULL,
  `arenaDifficulty` int(11) NOT NULL,
  `worldFile` varchar(64) NOT NULL,
  `numTeams` int(11) NOT NULL,
  `team1Name` varchar(32) NOT NULL,
  `team1Size` int(11) NOT NULL,
  `team1Race` varchar(32) DEFAULT NULL,
  `team2Name` varchar(32) DEFAULT NULL,
  `team2Size` int(11) DEFAULT NULL,
  `team2Race` varchar(32) DEFAULT NULL,
  `team3Name` varchar(32) DEFAULT NULL,
  `team3Size` int(11) DEFAULT NULL,
  `team3Race` varchar(32) DEFAULT NULL,
  `team4Name` varchar(32) DEFAULT NULL,
  `team4Size` int(11) DEFAULT NULL,
  `team4Race` varchar(32) DEFAULT NULL,
  `victoryCurrency` int(11) DEFAULT NULL,
  `victoryPayment` int(11) DEFAULT NULL,
  `defeatCurrency` int(11) DEFAULT NULL,
  `defeatPayment` int(11) DEFAULT NULL,
  `victoryExp` int(11) DEFAULT NULL,
  `defeatExp` int(11) DEFAULT NULL,
  `length` int(11) DEFAULT NULL,
  `team1Goal` int(11) DEFAULT NULL,
  `team2Goal` int(11) DEFAULT NULL,
  `team3Goal` int(11) DEFAULT NULL,
  `team4Goal` int(11) DEFAULT NULL,
  `victoryCondition` int(11) DEFAULT NULL,
  `raceOption1` varchar(32) DEFAULT NULL,
  `raceOption2` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `arena_templates`
--

LOCK TABLES `arena_templates` WRITE;
/*!40000 ALTER TABLE `arena_templates` DISABLE KEYS */;
/*!40000 ALTER TABLE `arena_templates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `aspect`
--

DROP TABLE IF EXISTS `aspect`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `aspect` (
  `id` int(11) NOT NULL,
  `name` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name_UNIQUE` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `aspect`
--

LOCK TABLES `aspect` WRITE;
/*!40000 ALTER TABLE `aspect` DISABLE KEYS */;
INSERT INTO `aspect` VALUES (2,'Mage'),(3,'Rogue'),(1,'Warrior');
/*!40000 ALTER TABLE `aspect` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `building_grids`
--

DROP TABLE IF EXISTS `building_grids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `building_grids` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `instance` varchar(45) NOT NULL,
  `locX` float DEFAULT NULL,
  `locY` float DEFAULT NULL,
  `locZ` float DEFAULT NULL,
  `type` int(11) DEFAULT NULL,
  `owner` bigint(20) DEFAULT NULL,
  `layer_count` int(11) DEFAULT '1',
  `building1` varchar(45) DEFAULT NULL,
  `building1_rotation` float DEFAULT NULL,
  `building2` varchar(45) DEFAULT NULL,
  `building2_rotation` float DEFAULT NULL,
  `building3` varchar(45) DEFAULT NULL,
  `building3_rotation` float DEFAULT NULL,
  `building4` varchar(45) DEFAULT NULL,
  `building4_rotation` float DEFAULT NULL,
  `building5` varchar(45) DEFAULT NULL,
  `building5_rotation` float DEFAULT NULL,
  `layer_height` float DEFAULT NULL,
  `blueprint1` int(11) DEFAULT '-1',
  `blueprint2` int(11) DEFAULT '-1',
  `blueprint3` int(11) DEFAULT '-1',
  `blueprint4` int(11) DEFAULT '-1',
  `blueprint5` int(11) DEFAULT '-1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `building_grids`
--

LOCK TABLES `building_grids` WRITE;
/*!40000 ALTER TABLE `building_grids` DISABLE KEYS */;
/*!40000 ALTER TABLE `building_grids` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_create_items`
--

DROP TABLE IF EXISTS `character_create_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `character_create_items` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `character_create_id` int(11) NOT NULL,
  `item_id` int(11) NOT NULL,
  `count` int(11) NOT NULL DEFAULT '1',
  `equipped` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `character_create_items`
--

LOCK TABLES `character_create_items` WRITE;
/*!40000 ALTER TABLE `character_create_items` DISABLE KEYS */;
/*!40000 ALTER TABLE `character_create_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_create_skills`
--

DROP TABLE IF EXISTS `character_create_skills`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `character_create_skills` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `character_create_id` int(11) NOT NULL,
  `skill` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `character_create_skills`
--

LOCK TABLES `character_create_skills` WRITE;
/*!40000 ALTER TABLE `character_create_skills` DISABLE KEYS */;
INSERT INTO `character_create_skills` VALUES (1,1,1),(2,1,2);
/*!40000 ALTER TABLE `character_create_skills` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_create_stats`
--

DROP TABLE IF EXISTS `character_create_stats`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `character_create_stats` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `character_create_id` int(11) NOT NULL,
  `stat` varchar(45) NOT NULL,
  `value` int(11) NOT NULL,
  `levelIncrease` FLOAT NOT NULL DEFAULT 0,
  `levelPercentIncrease` FLOAT NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `character_create_stats`
--

LOCK TABLES `character_create_stats` WRITE;
/*!40000 ALTER TABLE `character_create_stats` DISABLE KEYS */;
INSERT INTO `character_create_stats` VALUES (1,1,'strength',20,1,0),(2,1,'dexterity',20,1,0),(3,1,'potential',20,1,0),(4,1,'intelligence',10,1,0),(5,1,'endurance',10,1,0),(6,1,'willpower',20,1,0),(7,1,'crush_resistance',5,1,0),(8,1,'slash_resistance',5,1,0),(9,1,'pierce_resistance',5,1,0),(10,1,'health',20,0,0),(11,1,'mana',20,0,0),(12,1,'movement_speed',7,0,0),(13,1,'attack_speed',2000,0,0);
/*!40000 ALTER TABLE `character_create_stats` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `character_create_template`
--

DROP TABLE IF EXISTS `character_create_template`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `character_create_template` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `race` varchar(45) NOT NULL,
  `aspect` varchar(45) NOT NULL,
  `instanceName` varchar(45) NOT NULL,
  `pos_x` float NOT NULL,
  `pos_y` float NOT NULL,
  `pos_z` float NOT NULL,
  `orientation` float NOT NULL,
  `faction` int(11) NOT NULL DEFAULT '1',
  `autoAttack` INT NOT NULL DEFAULT -1,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

ALTER TABLE character_create_template ADD UNIQUE `unique_index`(`race`, `aspect`);

--
-- Dumping data for table `character_create_template`
--

LOCK TABLES `character_create_template` WRITE;
/*!40000 ALTER TABLE `character_create_template` DISABLE KEYS */;
INSERT INTO `character_create_template` VALUES (1,'Human','Warrior','MainWorld',-128.985,-34,15.7763,1,1,1);
/*!40000 ALTER TABLE `character_create_template` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `claim`
--

DROP TABLE IF EXISTS `claim`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `claim` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NULL,
  `instance` varchar(45) NOT NULL,
  `locX` float NOT NULL,
  `locY` float NOT NULL,
  `locZ` float NOT NULL,
  `owner` bigint(20),
  `size` int(11) DEFAULT 30,
  `forSale` TINYINT(1) NULL DEFAULT 0,
  `cost` INT NULL DEFAULT 0,
  `currency` INT NULL,
  `claimItemTemplate` INT(11) NULL DEFAULT -1,
  `priority` INT(11) NOT NULL DEFAULT 1,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `claim_action`
--

DROP TABLE IF EXISTS `claim_action`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `claim_action` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `claimID` int(11),
  `action` varchar(45),
  `brushType` varchar(45) NOT NULL,
  `locX` float NOT NULL,
  `locY` float NOT NULL,
  `locZ` float NOT NULL,
  `material` smallint(6) NOT NULL,
  `normalX` float NOT NULL,
  `normalY` float NOT NULL,
  `normalZ` float NOT NULL,
  `sizeX` float NOT NULL,
  `sizeY` float NOT NULL,
  `sizeZ` float NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

CREATE TABLE `claim_object` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `claimID` INT NULL,
  `gameObject` varchar(256) NOT NULL,
  `locX` FLOAT NULL,
  `locY` FLOAT NULL,
  `locZ` FLOAT NULL,
  `orientX` FLOAT NULL,
  `orientY` FLOAT NULL,
  `orientZ` FLOAT NULL,
  `orientW` FLOAT NULL,
  `itemID` INT NULL,
  PRIMARY KEY (`id`));

--
-- Table structure for table `cooldown_effects`
--

DROP TABLE IF EXISTS `cooldown_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `cooldown_effects` (
  `id` int(11) NOT NULL,
  `cooldown` varchar(32) DEFAULT NULL,
  `cooldownLength` int(11) DEFAULT NULL,
  `resetCooldown` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `cooldown_effects`
--

LOCK TABLES `cooldown_effects` WRITE;
/*!40000 ALTER TABLE `cooldown_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `cooldown_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `coordinated_effects`
--

DROP TABLE IF EXISTS `coordinated_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `coordinated_effects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `prefab` varchar(256) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `coordinated_effects`
--

LOCK TABLES `coordinated_effects` WRITE;
/*!40000 ALTER TABLE `coordinated_effects` DISABLE KEYS */;
INSERT INTO `coordinated_effects` VALUES (2,'Attack Effect','Assets/Resources/Content/CoordinatedEffects/StandardMeleeAttack.prefab'),(3,'Attack Effect Special','Assets/Resources/Content/CoordinatedEffects/SpecialMeleeAttack.prefab'),(4,'Attack Effect Special 2','Assets/Resources/Content/CoordinatedEffects/SpecialMeleeAttack2.prefab'),(6,'Fireball','Assets/Resources/Content/CoordinatedEffects/FireballEffect.prefab');
/*!40000 ALTER TABLE `coordinated_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping data for table `crafting_recipes`
--

CREATE TABLE `crafting_recipes` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(64) NULL,
  `icon` VARCHAR(256) NULL,
  `resultItemID` INT NULL,
  `resultItemCount` INT NULL DEFAULT 1,
  `skillID` INT NULL,
  `skillLevelReq` INT NULL,
  `skillLevelMax` INT NULL,
  `stationReq` VARCHAR(45) NULL,
  `creationTime` INT NULL DEFAULT 0,
  `recipeItemID` INT NULL,
  `layoutReq` TINYINT(1) NULL DEFAULT 1,
  `qualityChangeable` TINYINT(1) NULL,
  `allowDyes` TINYINT(1) NULL,
  `allowEssences` TINYINT(1) NULL,
  `component1` INT NULL DEFAULT -1,
  `component1count` INT NULL,
  `component2` INT NULL DEFAULT -1,
  `component2count` INT NULL,
  `component3` INT NULL DEFAULT -1,
  `component3count` INT NULL,
  `component4` INT NULL DEFAULT -1,
  `component4count` INT NULL,
  `component5` INT NULL DEFAULT -1,
  `component5count` INT NULL,
  `component6` INT NULL DEFAULT -1,
  `component6count` INT NULL,
  `component7` INT NULL DEFAULT -1,
  `component7count` INT NULL,
  `component8` INT NULL DEFAULT -1,
  `component8count` INT NULL,
  `component9` INT NULL DEFAULT -1,
  `component9count` INT NULL,
  `component10` INT NULL DEFAULT -1,
  `component10count` INT NULL,
  `component11` INT NULL DEFAULT -1,
  `component11count` INT NULL,
  `component12` INT NULL DEFAULT -1,
  `component12count` INT NULL,
  `component13` INT NULL DEFAULT -1,
  `component13count` INT NULL,
  `component14` INT NULL DEFAULT -1,
  `component14count` INT NULL,
  `component15` INT NULL DEFAULT -1,
  `component15count` INT NULL,
  `component16` INT NULL DEFAULT -1,
  `component16count` INT NULL,
  PRIMARY KEY (`id`));

--
-- Table structure for table `create_item_effects`
--

DROP TABLE IF EXISTS `create_item_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `create_item_effects` (
  `id` int(11) NOT NULL,
  `item` int(11) NOT NULL DEFAULT '-1',
  `count` int(11) DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `create_item_effects`
--

LOCK TABLES `create_item_effects` WRITE;
/*!40000 ALTER TABLE `create_item_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `create_item_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `currencies`
--

DROP TABLE IF EXISTS `currencies`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `currencies` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `category` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `icon` VARCHAR(256) NOT NULL,
  `description` varchar(225) DEFAULT NULL,
  `maximum` INT(11) NOT NULL DEFAULT 999999,
  `external` tinyint(1) DEFAULT '0',
  `isSubCurrency` tinyint(1) DEFAULT '0',
  `subCurrency1` INT(11) DEFAULT -1,
  `subCurrency2` INT(11) DEFAULT -1,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=2 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `currencies`
--

LOCK TABLES `currencies` WRITE;
/*!40000 ALTER TABLE `currencies` DISABLE KEYS */;
INSERT INTO `currencies` VALUES (1,1,'Gold','','',999999,0,0,-1,-1);
/*!40000 ALTER TABLE `currencies` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `damage_effects`
--

DROP TABLE IF EXISTS `damage_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `damage_effects` (
  `id` int(11) NOT NULL,
  `damageAmount` int(11) DEFAULT NULL,
  `damageType` varchar(32) DEFAULT NULL,
  `damageProperty` varchar(32) NOT NULL,
  `damageMod` float DEFAULT NULL,
  `bonusDamageEffect` int(11) DEFAULT NULL,
  `bonusDamageAmount` int(11) DEFAULT NULL,
  `healthTransferRate` float DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `damage_effects`
--

LOCK TABLES `damage_effects` WRITE;
/*!40000 ALTER TABLE `damage_effects` DISABLE KEYS */;
INSERT INTO `damage_effects` VALUES (1,20,'crush','health',1,-1,0,0),(4,20,'crush','health',1,-1,0,0);
/*!40000 ALTER TABLE `damage_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `damage_mitigation_effects`
--

DROP TABLE IF EXISTS `damage_mitigation_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `damage_mitigation_effects` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `mitigationType` int(11) NOT NULL DEFAULT '0',
  `mitigationValueType` int(11) NOT NULL DEFAULT '0',
  `amountMitigated` int(11) NOT NULL DEFAULT '1',
  `attacksMitigated` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `damage_mitigation_effects`
--

LOCK TABLES `damage_mitigation_effects` WRITE;
/*!40000 ALTER TABLE `damage_mitigation_effects` DISABLE KEYS */;
INSERT INTO `damage_mitigation_effects` VALUES (6,0,0,5,1);
/*!40000 ALTER TABLE `damage_mitigation_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `damage_type`
--

DROP TABLE IF EXISTS `damage_type`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `damage_type` (
  `name` varchar(45) NOT NULL,
  `resistance_stat` varchar(45) NOT NULL,
  PRIMARY KEY (`name`),
  UNIQUE KEY `name_UNIQUE` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `damage_type`
--

LOCK TABLES `damage_type` WRITE;
/*!40000 ALTER TABLE `damage_type` DISABLE KEYS */;
INSERT INTO `damage_type` VALUES ('crush','crush_resistance'),('pierce','pierce_resistance'),('slash','slash_resistance');
/*!40000 ALTER TABLE `damage_type` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `despawn_effects`
--

DROP TABLE IF EXISTS `despawn_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `despawn_effects` (
  `id` int(11) NOT NULL,
  `mobID` int(11) NOT NULL DEFAULT '-1',
  `despawnType` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `dialogue`
--

DROP TABLE IF EXISTS `dialogue`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `dialogue` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(45) DEFAULT NULL,
  `openingDialogue` tinyint(1) DEFAULT '1',
  `repeatable` tinyint(1) DEFAULT '0',
  `prereqDialogue` int(11) DEFAULT '-1',
  `prereqQuest` int(11) DEFAULT '-1',
  `prereqFaction` int(11) DEFAULT '-1',
  `prereqFactionStance` int(11) DEFAULT '1',
  `reactionAutoStart` tinyint(1) DEFAULT '0',
  `text` text,
  `option1text` varchar(256) DEFAULT NULL,
  `option1action` varchar(45) DEFAULT NULL,
  `option1actionID` int(11) DEFAULT NULL,
  `option2text` varchar(256) DEFAULT NULL,
  `option2action` varchar(45) DEFAULT NULL,
  `option2actionID` int(11) DEFAULT NULL,
  `option3text` varchar(256) DEFAULT NULL,
  `option3action` varchar(45) DEFAULT NULL,
  `option3actionID` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `despawn_effects`
--

LOCK TABLES `despawn_effects` WRITE;
/*!40000 ALTER TABLE `despawn_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `despawn_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `effects`
--

DROP TABLE IF EXISTS `effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `effects` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `displayName` varchar(64) DEFAULT NULL,
  `icon` varchar(64) DEFAULT NULL,
  `effectMainType` varchar(64) DEFAULT NULL,
  `effectType` varchar(64) DEFAULT NULL,
  `isBuff` tinyint(1) NOT NULL DEFAULT '0',
  `skillType` int(11) DEFAULT NULL,
  `skillLevelMod` float DEFAULT '0',
  `passive` tinyint(1) DEFAULT NULL,
  `stackLimit` int(11) DEFAULT NULL,
  `allowMultiple` tinyint(1) DEFAULT NULL,
  `duration` float DEFAULT NULL,
  `pulseCount` int(11) DEFAULT NULL,
  `tooltip` varchar(255) DEFAULT NULL,
  `bonusEffectReq` int(11) NOT NULL DEFAULT '-1',
  `bonusEffectReqConsumed` tinyint(1) DEFAULT NULL,
  `bonusEffect` int(11) NOT NULL DEFAULT '-1',
  `pulseParticle` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `effects`
--

LOCK TABLES `effects` WRITE;
/*!40000 ALTER TABLE `effects` DISABLE KEYS */;
INSERT INTO `effects` VALUES (1,'player attack effect',NULL,'','Damage','MeleeStrikeEffect',0,-1,0,0,0,0,1,1,'',-1,0,-1,''),(4,'Fireball Damage',NULL,'','Damage','MagicalStrikeEffect',0,-1,0,0,0,0,0,1,'',-1,0,-1,'');
/*!40000 ALTER TABLE `effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `events`
--

DROP TABLE IF EXISTS `events`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `events` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) NOT NULL,
  `data_type` varchar(32) DEFAULT NULL,
  `save_data` tinyint(1) NOT NULL DEFAULT '0',
  `description` varchar(256) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `events`
--

LOCK TABLES `events` WRITE;
/*!40000 ALTER TABLE `events` DISABLE KEYS */;
INSERT INTO `events` VALUES (1,'Level Up',NULL,0,'Called when a player levels up'),(2,'Mob Death','ID',1,'Sends through the template ID of the mob killed'),(3,'Player Death',NULL,1,'A player died'),(4,'Item Looted','ID',1,'A player picks up an item. Can be from either a loot bag a dead mob dropped or a random spawned item'),(5,'Item Purchased','ID',1,'A player purchases an item'),(6,'Currency Looted','ID',1,'Currency is looted by the player'),(7,'Item Sold','ID',1,'A player sells an item'),(9,'Log out',NULL,0,'Called when a player logs out. Can be used to reset achievements that require the player complete during a single login session'),(10,'Arena Completed','Category',1,'Sends through the category of the arena when upon completion. Often used to unlock skins through achievements'),(11,'Achievement Completed','ID',1,'Used so other achievements can be activated'),(12,'Muncher Victory',NULL,1,'Victory in the Muncher Arena'),(13,'Muncher Defeat',NULL,1,'Defeat in the Muncher Arena'),(14,'Muncher Completed',NULL,1,'Muncher Arena Completed'),(15,'Bomber Victory',NULL,1,'Victory in the Bomber Arena'),(16,'Bomber Defeat',NULL,1,'Defeat in the Bomber Arena'),(17,'Bomber Completed',NULL,1,'Bomber Arena Completed'),(18,'CTF Victory',NULL,1,'Victory in the CTF Arena'),(19,'CTF Defeat',NULL,1,'Defeat in the CTF Arena'),(20,'CTF Completed',NULL,1,'CTF Arena Completed');
/*!40000 ALTER TABLE `events` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `faction_effects`
--

DROP TABLE IF EXISTS `faction_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `faction_effects` (
  `id` int(11) NOT NULL,
  `faction` int(11) NOT NULL DEFAULT '-1',
  `repLevel` varchar(32) DEFAULT NULL,
  `repLevelDefault` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `faction_effects`
--

LOCK TABLES `faction_effects` WRITE;
/*!40000 ALTER TABLE `faction_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `faction_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `faction_stances`
--

DROP TABLE IF EXISTS `faction_stances`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `faction_stances` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `factionID` int(11) NOT NULL,
  `otherFaction` int(11) NOT NULL DEFAULT '-1',
  `defaultStance` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `factions`
--

DROP TABLE IF EXISTS `factions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `factions` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `category` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `factionGroup` varchar(64) DEFAULT NULL,
  `public` tinyint(1) NOT NULL DEFAULT '0',
  `defaultStance` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=MyISAM AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `factions`
--

LOCK TABLES `factions` WRITE;
/*!40000 ALTER TABLE `factions` DISABLE KEYS */;
INSERT INTO `factions` VALUES (1,1,'Human','',1,0),(2,1,'Haters',NULL,0,-2),(3,1,'Friendly',NULL,0,1),(4,1,'Neutral',NULL,0,0);
/*!40000 ALTER TABLE `factions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `heal_effects`
--

DROP TABLE IF EXISTS `heal_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `heal_effects` (
  `id` int(11) NOT NULL,
  `healAmount` int(11) DEFAULT NULL,
  `healProperty` varchar(32) DEFAULT NULL,
  `healthTransferRate` float DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `heal_effects`
--

LOCK TABLES `heal_effects` WRITE;
/*!40000 ALTER TABLE `heal_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `heal_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `item_templates`
--

DROP TABLE IF EXISTS `item_templates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `item_templates` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `icon` varchar(256) DEFAULT NULL,
  `category` varchar(64) DEFAULT NULL,
  `subcategory` varchar(64) DEFAULT NULL,
  `itemType` varchar(64) DEFAULT NULL,
  `subType` varchar(64) DEFAULT NULL,
  `slot` varchar(64) DEFAULT NULL,
  `display` varchar(128) DEFAULT NULL,
  `itemQuality` tinyint(11) DEFAULT NULL,
  `binding` tinyint(11) DEFAULT NULL,
  `isUnique` tinyint(1) DEFAULT NULL,
  `stackLimit` int(11) DEFAULT NULL,
  `duration` int(11) DEFAULT NULL,
  `purchaseCurrency` tinyint(11) DEFAULT NULL,
  `purchaseCost` int(11) DEFAULT NULL,
  `sellable` tinyint(1) DEFAULT '1',
  `levelReq` int(11) DEFAULT NULL,
  `aspectReq` varchar(64) DEFAULT NULL,
  `raceReq` varchar(64) DEFAULT NULL,
  `damage` int(11) NOT NULL DEFAULT '0',
  `damageType` varchar(32) DEFAULT NULL,
  `delay` float DEFAULT NULL,
  `toolTip` varchar(255) DEFAULT NULL,
  `triggerEvent` varchar(32) DEFAULT NULL,
  `triggerAction1Type` varchar(32) DEFAULT NULL,
  `triggerAction1Data` varchar(32) DEFAULT NULL,
  `effect1type` VARCHAR(32) DEFAULT NULL,
  `effect1name` VARCHAR(45) NULL,
  `effect1value` VARCHAR(256) DEFAULT '0',
  `effect2type` VARCHAR(32) DEFAULT NULL,
  `effect2name` VARCHAR(45) NULL,
  `effect2value` VARCHAR(256) DEFAULT '0',
  `effect3type` VARCHAR(32) DEFAULT NULL,
  `effect3name` VARCHAR(45) NULL,
  `effect3value` VARCHAR(256) DEFAULT '0',
  `effect4type` VARCHAR(32) DEFAULT NULL,
  `effect4name` VARCHAR(45) NULL,
  `effect4value` VARCHAR(256) DEFAULT '0',
  `effect5type` VARCHAR(32) DEFAULT NULL,
  `effect5name` VARCHAR(45) NULL,
  `effect5value` VARCHAR(256) DEFAULT '0',
  `effect6type` VARCHAR(32) DEFAULT NULL,
  `effect6name` VARCHAR(45) NULL,
  `effect6value` VARCHAR(256) DEFAULT '0',
  `effect7type` VARCHAR(32) DEFAULT NULL,
  `effect7name` VARCHAR(45) NULL,
  `effect7value` VARCHAR(256) DEFAULT '0',
  `effect8type` VARCHAR(32) DEFAULT NULL,
  `effect8name` VARCHAR(45) NULL,
  `effect8value` VARCHAR(256) DEFAULT '0',
  `effect9type` VARCHAR(32) DEFAULT NULL,
  `effect9name` VARCHAR(45) NULL,
  `effect9value` VARCHAR(256) DEFAULT '0',
  `effect10type` VARCHAR(32) DEFAULT NULL,
  `effect10name` VARCHAR(45) NULL,
  `effect10value` VARCHAR(256) DEFAULT '0',
  `effect11type` VARCHAR(32) DEFAULT NULL,
  `effect11name` VARCHAR(45) NULL,
  `effect11value` VARCHAR(256) DEFAULT '0',
  `effect12type` VARCHAR(32) DEFAULT NULL,
  `effect12name` VARCHAR(45) NULL,
  `effect12value` VARCHAR(256) DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `item_templates`
--

LOCK TABLES `item_templates` WRITE;
/*!40000 ALTER TABLE `item_templates` DISABLE KEYS */;
INSERT INTO `item_templates` VALUES (1,'Pink Star','Assets/Characters/Princess/FBX/wand.fbm/weapons_princess1_1.dds','0','0','Material','Sword','Main Hand','',1,0,0,1,NULL,1,0,0,0,NULL,'0',0,'Slash',1.5,'',NULL,NULL,NULL,NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0'),(2,'Wooden Stick','Assets/AGF_SourceAssets/MMO_Sample/Warehouse/Textures/Wood_Wall_01_Long_c.asset','0','0','Material','Sword','Main Hand','',1,0,0,99,NULL,1,0,0,0,NULL,'0',0,'Slash',1.5,'',NULL,NULL,NULL,NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0',NULL,NULL,'0'),(3,'Princess Wand','Assets/Characters/Princess/FBX/wand.fbm/weapons_princess1_1.dds','0','0','Weapon','Staff','Main Hand','Assets/Resources/Content/EquipmentDisplay/Princess Wand.prefab',2,0,0,1,NULL,1,0,0,0,NULL,'0',10,'crush',1.5,'',NULL,NULL,NULL,'Stat','endurance','5','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','');
/*!40000 ALTER TABLE `item_templates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `itemweights`
--

DROP TABLE IF EXISTS `item_weights`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `item_weights` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `prefix` tinyint(1) DEFAULT NULL,
  `stat1` varchar(64) DEFAULT NULL,
  `weight1` int(11) DEFAULT NULL,
  `stat2` varchar(64) DEFAULT NULL,
  `weight2` int(11) DEFAULT NULL,
  `stat3` varchar(64) DEFAULT NULL,
  `weight3` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=20 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `level_xp_requirements`
--

DROP TABLE IF EXISTS `level_xp_requirements`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `level_xp_requirements` (
  `level` int(11) NOT NULL,
  `xpRequired` int(11) DEFAULT NULL,
  PRIMARY KEY (`level`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `level_xp_requirements`
--

LOCK TABLES `level_xp_requirements` WRITE;
/*!40000 ALTER TABLE `level_xp_requirements` DISABLE KEYS */;
INSERT INTO `level_xp_requirements` VALUES (1,200),(2,500),(3,800),(4,1100),(5,1400),(6,1700),(7,2000);
/*!40000 ALTER TABLE `level_xp_requirements` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `loot_tables`
--

DROP TABLE IF EXISTS `loot_tables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `loot_tables` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `item1` int(11) NOT NULL DEFAULT '-1',
  `item1count` int(11) DEFAULT NULL,
  `item1chance` int(11) DEFAULT NULL,
  `item2` int(11) NOT NULL DEFAULT '-1',
  `item2count` int(11) DEFAULT NULL,
  `item2chance` int(11) DEFAULT NULL,
  `item3` int(11) NOT NULL DEFAULT '-1',
  `item3count` int(11) DEFAULT NULL,
  `item3chance` int(11) DEFAULT NULL,
  `item4` int(11) NOT NULL DEFAULT '-1',
  `item4count` int(11) DEFAULT NULL,
  `item4chance` int(11) DEFAULT NULL,
  `item5` int(11) NOT NULL DEFAULT '-1',
  `item5count` int(11) DEFAULT NULL,
  `item5chance` int(11) DEFAULT NULL,
  `item6` int(11) NOT NULL DEFAULT '-1',
  `item6count` int(11) DEFAULT NULL,
  `item6chance` int(11) DEFAULT NULL,
  `item7` int(11) NOT NULL DEFAULT '-1',
  `item7count` int(11) DEFAULT NULL,
  `item7chance` int(11) DEFAULT NULL,
  `item8` int(11) NOT NULL DEFAULT '-1',
  `item8count` int(11) DEFAULT NULL,
  `item8chance` int(11) DEFAULT NULL,
  `item9` int(11) NOT NULL DEFAULT '-1',
  `item9count` int(11) DEFAULT NULL,
  `item9chance` int(11) DEFAULT NULL,
  `item10` int(11) NOT NULL DEFAULT '-1',
  `item10count` int(11) DEFAULT NULL,
  `item10chance` int(11) DEFAULT NULL,
  `category` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`),
  KEY `item1` (`item1`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `merchant_tables`
--

DROP TABLE IF EXISTS `merchant_tables`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `merchant_tables` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=MyISAM AUTO_INCREMENT=1 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

CREATE TABLE `merchant_item` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `tableID` INT NOT NULL,
  `itemID` INT NOT NULL,
  `count` INT NULL,
  `refreshTime` INT NULL,
  PRIMARY KEY (`id`)
);

--
-- Table structure for table `message_effects`
--

DROP TABLE IF EXISTS `message_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `message_effects` (
  `id` int(11) NOT NULL,
  `messageType` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `message_effects`
--

LOCK TABLES `message_effects` WRITE;
/*!40000 ALTER TABLE `message_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `message_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `mob_display`
--

DROP TABLE IF EXISTS `mob_display`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mob_display` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `prefab` varchar(64) NOT NULL,
  `race` varchar(64) DEFAULT NULL,
  `gender` varchar(32) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `mob_display`
--

LOCK TABLES `mob_display` WRITE;
/*!40000 ALTER TABLE `mob_display` DISABLE KEYS */;
/*!40000 ALTER TABLE `mob_display` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `mob_loot`
--

DROP TABLE IF EXISTS `mob_loot`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mob_loot` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `category` int(11) NOT NULL DEFAULT '1',
  `mobTemplate` int(11) NOT NULL,
  `lootTable` int(11) DEFAULT NULL,
  `dropChance` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `mobTemplate` (`mobTemplate`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `mobtemplates`
--

DROP TABLE IF EXISTS `mob_templates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `mob_templates` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `category` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `subTitle` varchar(64) DEFAULT NULL,
  `mobType` int(11) NOT NULL,
  `display1` varchar(128) NOT NULL DEFAULT '-1',
  `display2` varchar(128) DEFAULT NULL,
  `display3` varchar(128) DEFAULT NULL,
  `display4` varchar(128) DEFAULT NULL,
  `scale` float DEFAULT NULL,
  `hitbox` int(11) DEFAULT NULL,
  `baseAnimationState` int(11) NOT NULL DEFAULT '1',
  `faction` int(11) NOT NULL DEFAULT '0',
  `attackable` tinyint(1) NOT NULL,
  `minLevel` int(11) NOT NULL,
  `maxLevel` int(11) DEFAULT NULL,
  `species` varchar(64) NOT NULL,
  `subSpecies` varchar(64) NOT NULL,
  `questCategory` varchar(32) DEFAULT NULL,
  `specialUse` varchar(32) DEFAULT NULL,
  `speed_walk` float DEFAULT NULL,
  `speed_run` float DEFAULT NULL,
  `minDmg` int(11) DEFAULT NULL,
  `maxDmg` int(11) DEFAULT NULL,
  `attackSpeed` float DEFAULT NULL,
  `dmgType` varchar(20) DEFAULT NULL,
  `primaryWeapon` int(11) DEFAULT NULL,
  `secondaryWeapon` int(11) DEFAULT NULL,
  `autoAttack` INT NOT NULL DEFAULT -1,
  PRIMARY KEY (`id`),
  UNIQUE KEY `name` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `npcdisplay`
--

DROP TABLE IF EXISTS `npcdisplay`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `npcdisplay` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `race` varchar(64) DEFAULT NULL,
  `gender` varchar(32) NOT NULL,
  `skinColour` int(11) NOT NULL DEFAULT '1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `npcdisplay`
--

LOCK TABLES `npcdisplay` WRITE;
/*!40000 ALTER TABLE `npcdisplay` DISABLE KEYS */;
INSERT INTO `npcdisplay` VALUES (1,'Red Smoo','Smoo','Male',1),(2,'Blue Smoo','Smoo','Male',2),(3,'Yellow Smoo','Smoo','Male',3),(4,'Green Smoo','Smoo','Male',4),(5,'Red Valkyrie','Valkyrie','Male',1),(6,'Blue Valkyrie','Valkyrie','Male',2),(7,'Yellow Valkyrie','Valkyrie','Male',3),(8,'Green Valkyrie','Valkyrie','Male',4),(10,'Red Robot','Robot','Male',1),(11,'Blue Robot','Robot','Male',2),(12,'Yellow Robot','Robot','Male',3),(13,'Red Viking','Viking','Male',1),(14,'Green Robot','Robot','Male',4),(15,'Yellow Ice Cream','Ice Cream','Male',3),(16,'Blue Ice Cream','Ice Cream','Male',2),(17,'Red Ice Cream','Ice Cream','Male',1),(19,'Pine Tree','Pine Tree','Male',1),(20,'Red Knight','Knight','Male',1);
/*!40000 ALTER TABLE `npcdisplay` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `patrolpaths`
--

DROP TABLE IF EXISTS `patrol_paths`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `patrol_paths` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(32) NOT NULL,
  `baseMarker` varchar(32) NOT NULL,
  `firstMarkerNum` int(11) NOT NULL,
  `lastMarkerNum` int(11) NOT NULL,
  `travelReverse` tinyint(1) NOT NULL,
  `pauseDuration` int(11) DEFAULT NULL,
  `pauseSpot1` int(11) DEFAULT NULL,
  `pauseSpot2` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=MyISAM AUTO_INCREMENT=9 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `patrolpaths`
--

LOCK TABLES `patrol_paths` WRITE;
/*!40000 ALTER TABLE `patrol_paths` DISABLE KEYS */;
/*!40000 ALTER TABLE `patrol_paths` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `property_effects`
--

DROP TABLE IF EXISTS `property_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `property_effects` (
  `id` int(11) NOT NULL,
  `property` varchar(32) DEFAULT NULL,
  `propertyType` varchar(32) DEFAULT NULL,
  `propertyValue` varchar(32) DEFAULT NULL,
  `propertyDefault` varchar(32) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `property_effects`
--

LOCK TABLES `property_effects` WRITE;
/*!40000 ALTER TABLE `property_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `property_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `quest_objectives`
--

DROP TABLE IF EXISTS `quest_objectives`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quest_objectives` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `questID` int(11) NOT NULL,
  `primaryObjective` tinyint(1) NOT NULL,
  `objectiveType` varchar(16) NOT NULL,
  `target` int(11) NOT NULL DEFAULT '-1',
  `targetCount` int(11) NOT NULL,
  `targetText` varchar(64) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `quests`
--

DROP TABLE IF EXISTS `quests`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `quests` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `category` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `faction` int(11) NOT NULL,
  `chain` varchar(64) DEFAULT NULL,
  `level` int(11) DEFAULT NULL,
  `zone` varchar(64) DEFAULT NULL,
  `numGrades` int(11) NOT NULL,
  `repeatable` tinyint(1) NOT NULL,
  `description` varchar(512) NOT NULL,
  `objectiveText` varchar(512) NOT NULL,
  `progressText` varchar(512) NOT NULL,
  `deliveryItem1` int(11) NOT NULL DEFAULT '-1',
  `deliveryItem2` int(11) NOT NULL DEFAULT '-1',
  `deliveryItem3` int(11) NOT NULL DEFAULT '-1',
  `questPrereq` int(11) NOT NULL DEFAULT '-1',
  `questStartedReq` int(11) NOT NULL DEFAULT '-1',
  `levelReq` int(11) DEFAULT NULL,
  `raceReq` varchar(32) DEFAULT NULL,
  `aspectReq` varchar(32) DEFAULT NULL,
  `skillReq` int(11) DEFAULT NULL,
  `skillLevelReq` int(11) DEFAULT NULL,
  `repReq` varchar(64) DEFAULT NULL,
  `repLevelReq` int(11) DEFAULT NULL,
  `completionText` varchar(512) DEFAULT NULL,
  `experience` int(11) DEFAULT NULL,
  `item1` int(11) DEFAULT NULL,
  `item1count` int(11) DEFAULT NULL,
  `item2` int(11) DEFAULT NULL,
  `item2count` int(11) DEFAULT NULL,
  `item3` int(11) DEFAULT NULL,
  `item3count` int(11) DEFAULT NULL,
  `item4` int(11) DEFAULT NULL,
  `item4count` int(11) DEFAULT NULL,
  `chooseItem1` int(11) DEFAULT NULL,
  `chooseItem1count` int(11) DEFAULT NULL,
  `chooseItem2` int(11) DEFAULT NULL,
  `chooseItem2count` int(11) DEFAULT NULL,
  `chooseItem3` int(11) DEFAULT NULL,
  `chooseItem3count` int(11) DEFAULT NULL,
  `chooseItem4` int(11) DEFAULT NULL,
  `chooseItem4count` int(11) DEFAULT NULL,
  `currency1` int(11) DEFAULT NULL,
  `currency1count` int(11) DEFAULT NULL,
  `currency2` int(11) DEFAULT NULL,
  `currency2count` int(11) DEFAULT NULL,
  `rep1` int(11) DEFAULT NULL,
  `rep1gain` int(11) DEFAULT NULL,
  `rep2` int(11) DEFAULT NULL,
  `rep2gain` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `quests`
--

LOCK TABLES `quests` WRITE;
/*!40000 ALTER TABLE `quests` DISABLE KEYS */;
/*!40000 ALTER TABLE `quests` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `resource_grids`
--

DROP TABLE IF EXISTS `resource_grids`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `resource_grids` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` varchar(45) DEFAULT NULL,
  `count` int(11) DEFAULT NULL,
  `locX` float DEFAULT NULL,
  `locY` float DEFAULT NULL,
  `locZ` float DEFAULT NULL,
  `rotation` float DEFAULT NULL,
  `instance` varchar(45) DEFAULT NULL,
  `resource1_type` varchar(45) DEFAULT NULL,
  `resource1_chance` int(11) DEFAULT NULL,
  `resource2_type` varchar(45) DEFAULT NULL,
  `resource2_chance` int(11) DEFAULT NULL,
  `resource3_type` varchar(45) DEFAULT NULL,
  `resource3_chance` int(11) DEFAULT NULL,
  `resource4_type` varchar(45) DEFAULT NULL,
  `resource4_chance` int(11) DEFAULT NULL,
  `resource5_type` varchar(45) DEFAULT NULL,
  `resource5_chance` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `resource_grids`
--

LOCK TABLES `resource_grids` WRITE;
/*!40000 ALTER TABLE `resource_grids` DISABLE KEYS */;
/*!40000 ALTER TABLE `resource_grids` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `skill_ability_gain`
--

DROP TABLE IF EXISTS `skill_ability_gain`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `skill_ability_gain` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `skillID` int(11) DEFAULT NULL,
  `skillLevelReq` int(11) NULL DEFAULT 1,
  `abilityID` int(11) DEFAULT NULL,
  `automaticallyLearn` TINYINT(1) NULL DEFAULT 1,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `skill_ability_gain`
--

LOCK TABLES `skill_ability_gain` WRITE;
/*!40000 ALTER TABLE `skill_ability_gain` DISABLE KEYS */;
INSERT INTO `skill_ability_gain` VALUES (1,2,1,4,1);
/*!40000 ALTER TABLE `skill_ability_gain` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `skills`
--

DROP TABLE IF EXISTS `skills`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `skills` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(64) NOT NULL,
  `icon` varchar(256) DEFAULT NULL,
  `aspect` varchar(45) DEFAULT NULL,
  `oppositeAspect` varchar(45) DEFAULT NULL,
  `primaryStat` varchar(45) NOT NULL,
  `secondaryStat` varchar(45) NOT NULL,
  `thirdStat` varchar(45) NOT NULL,
  `fourthStat` varchar(45) NOT NULL,
  `maxLevel` INT NULL DEFAULT 1,
  `automaticallyLearn` TINYINT(1) NULL DEFAULT 1,
  `skillPointCost` INT NULL DEFAULT 0,
  `parentSkill` int(11) NULL DEFAULT 0,
  `parentSkillLevelReq` INT NULL DEFAULT 1, 
  `prereqSkill1` INT NULL DEFAULT 0,
  `prereqSkill1Level` INT NULL DEFAULT 1,
  `prereqSkill2` INT NULL DEFAULT 0,
  `prereqSkill2Level` INT NULL DEFAULT 1,
  `prereqSkill3` INT NULL DEFAULT 0,
  `prereqSkill3Level` INT NULL DEFAULT 1,
  `playerLevelReq` INT NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `skills`
--

LOCK TABLES `skills` WRITE;
/*!40000 ALTER TABLE `skills` DISABLE KEYS */;
INSERT INTO `skills` VALUES (1,'Hammer Swing',NULL,'Warrior','Mage','strength','endurance','dexterity','willpower',1,1,0,0,1,0,1,0,1,0,1,1),(2,'Destruction',NULL,'Mage','Warrior','intelligence','willpower','potential','dexterity',1,1,0,0,1,0,1,0,1,0,1,1);
/*!40000 ALTER TABLE `skills` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `spawn_effects`
--

DROP TABLE IF EXISTS `spawn_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `spawn_effects` (
  `id` int(11) NOT NULL,
  `mobID` int(11) NOT NULL DEFAULT '-1',
  `spawnType` int(11) NOT NULL DEFAULT '0',
  `passiveEffect` int(11) NOT NULL DEFAULT '-1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `spawn_effects`
--

LOCK TABLES `spawn_effects` WRITE;
/*!40000 ALTER TABLE `spawn_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `spawn_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `spawn_data`
--

DROP TABLE IF EXISTS `spawn_data`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `spawn_data` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `category` int(11) NOT NULL DEFAULT '1',
  `name` varchar(64) NOT NULL,
  `mobTemplate` int(11) NOT NULL DEFAULT '-1',
  `markerName` varchar(64) DEFAULT NULL,
  `locX` int(11) DEFAULT NULL,
  `locY` int(11) DEFAULT NULL,
  `locZ` int(11) DEFAULT NULL,
  `orientX` float(8,4) DEFAULT NULL,
  `orientY` float(8,4) DEFAULT NULL,
  `orientZ` float(8,4) DEFAULT NULL,
  `orientW` float(8,4) DEFAULT NULL,
  `instance` varchar(64) DEFAULT NULL,
  `numSpawns` int(11) DEFAULT NULL,
  `spawnRadius` int(11) DEFAULT NULL,
  `respawnTime` int(11) DEFAULT NULL,
  `corpseDespawnTime` int(11) DEFAULT NULL,
  `combat` tinyint(1) NOT NULL,
  `roamRadius` int(11) NOT NULL,
  `startsQuests` varchar(256) NOT NULL,
  `endsQuests` varchar(256) NOT NULL,
  `startsDialogues` varchar(256) NOT NULL,
  `baseAction` varchar(32) NOT NULL,
  `weaponSheathed` tinyint(1) NOT NULL,
  `merchantTable` int(11) NOT NULL,
  `questOpenLootTable` int(11) NOT NULL,
  `isChest` tinyint(4) NOT NULL DEFAULT '0',
  `pickupItem` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `stat`
--

DROP TABLE IF EXISTS `stat`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `stat` (
  `name` varchar(45) NOT NULL,
  `type` int(11) DEFAULT '0',
  `stat_function` varchar(45) DEFAULT NULL,
  `mob_base` int(11) DEFAULT NULL,
  `mob_level_increase` int(11) DEFAULT NULL,
  `mob_level_percent_increase` float DEFAULT NULL,
  `min` int(11) NOT NULL DEFAULT '0',
  `maxstat` varchar(45) DEFAULT NULL,
  `shiftTarget` SMALLINT NULL DEFAULT '0',
  `shiftValue` int(11) DEFAULT NULL,
  `shiftReverseValue` int(11) DEFAULT NULL,
  `shiftInterval` int(11) DEFAULT NULL,
  `isShiftPercent` tinyint(1) NOT NULL DEFAULT '0',
  `onMaxHit` varchar(45) DEFAULT NULL,
  `onMinHit` varchar(45) DEFAULT NULL,
  `shiftReq1` varchar(45) DEFAULT NULL,
  `shiftReq1State` tinyint(1) NOT NULL DEFAULT '0',
  `shiftReq1SetReverse` tinyint(1) NOT NULL DEFAULT '0',
  `shiftReq2` varchar(45) DEFAULT NULL,
  `shiftReq2State` tinyint(1) NOT NULL DEFAULT '0',
  `shiftReq2SetReverse` tinyint(1) NOT NULL DEFAULT '0',
  `shiftReq3` varchar(45) DEFAULT NULL,
  `shiftReq3State` tinyint(1) NOT NULL DEFAULT '0',
  `shiftReq3SetReverse` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`name`),
  UNIQUE KEY `name_UNIQUE` (`name`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stat`
--

LOCK TABLES `stat` WRITE;
/*!40000 ALTER TABLE `stat` DISABLE KEYS */;
INSERT INTO `stat` VALUES ('crush_resistance',1,NULL,5,1,0,0,NULL,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0),
('dexterity',0,'Physical Accuracy',20,1,0,0,NULL,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0),
('endurance',0,'Health Mod',10,2,0,0,NULL,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0),
('health',2,'Health',0,0,0,0,'',0,3,0,2,1,NULL,NULL,'deadstate',0,0,'combatstate',0,0,NULL,0,0),
('intelligence',0,'Magical Accuracy',20,1,0,0,NULL,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0),
('mana',2,'Mana',0,0,0,0,'',0,3,0,2,1,NULL,NULL,'deadstate',0,0,NULL,0,0,NULL,0,0),
('movement_speed',0,NULL,7,0,0,0,NULL,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0),
('pierce_resistance',1,NULL,5,1,0,0,NULL,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0),
('potential',0,'Magical Power',20,1,0,0,NULL,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0),
('slash_resistance',1,NULL,5,1,0,0,NULL,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0),
('strength',0,'Physical Power',20,1,0,0,NULL,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0),
('willpower',0,'Mana Mod',10,2,0,0,NULL,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0),
('attack_speed',0,NULL,2000,0,0,1000,10000,0,0,0,0,0,NULL,NULL,NULL,0,0,NULL,0,0,NULL,0,0);
/*!40000 ALTER TABLE `stat` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stat_effects`
--

DROP TABLE IF EXISTS `stat_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `stat_effects` (
  `id` int(11) NOT NULL,
  `modifyStatsByPercent` TINYINT(1) DEFAULT NULL,
  `stat1Name` varchar(32) DEFAULT NULL,
  `stat1Modification` FLOAT DEFAULT NULL,
  `stat2Name` varchar(32) DEFAULT NULL,
  `stat2Modification` FLOAT DEFAULT NULL,
  `stat3Name` varchar(32) DEFAULT NULL,
  `stat3Modification` FLOAT DEFAULT NULL,
  `stat4Name` varchar(32) DEFAULT NULL,
  `stat4Modification` FLOAT DEFAULT NULL,
  `stat5Name` varchar(32) DEFAULT NULL,
  `stat5Modification` FLOAT DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stat_effects`
--

LOCK TABLES `stat_effects` WRITE;
/*!40000 ALTER TABLE `stat_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `stat_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stun_effects`
--

DROP TABLE IF EXISTS `stun_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `stun_effects` (
  `id` int(11) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `stun_effects`
--

LOCK TABLES `stun_effects` WRITE;
/*!40000 ALTER TABLE `stun_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `stun_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `survivalarenatemplates`
--

DROP TABLE IF EXISTS `survival_arenatemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `survivalarenatemplates` (
  `id` int(11) NOT NULL,
  `numRounds` int(11) NOT NULL,
  `round1Spawns` varchar(128) NOT NULL,
  `round2Spawns` varchar(128) DEFAULT NULL,
  `round3Spawns` varchar(128) DEFAULT NULL,
  `round4Spawns` varchar(128) DEFAULT NULL,
  `round5Spawns` varchar(128) DEFAULT NULL,
  `round6Spawns` varchar(128) DEFAULT NULL,
  `round7Spawns` varchar(128) DEFAULT NULL,
  `round8Spawns` varchar(128) DEFAULT NULL,
  `round9Spawns` varchar(128) DEFAULT NULL,
  `round10Spawns` varchar(128) DEFAULT NULL,
  PRIMARY KEY (`id`),
  CONSTRAINT `survivalArenaTemplates_ibfk_1` FOREIGN KEY (`id`) REFERENCES `arena_templates` (`id`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `survivalarenatemplates`
--

LOCK TABLES `survivalarenatemplates` WRITE;
/*!40000 ALTER TABLE `survivalarenatemplates` DISABLE KEYS */;
/*!40000 ALTER TABLE `survivalarenatemplates` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `task_effects`
--

DROP TABLE IF EXISTS `task_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `task_effects` (
  `id` int(11) NOT NULL,
  `task` int(11) NOT NULL DEFAULT '-1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `task_effects`
--

LOCK TABLES `task_effects` WRITE;
/*!40000 ALTER TABLE `task_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `task_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `teleport_effects`
--

DROP TABLE IF EXISTS `teleport_effects`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `teleport_effects` (
  `id` int(11) NOT NULL,
  `marker` varchar(64) DEFAULT NULL,
  `locX` float DEFAULT NULL,
  `locY` float DEFAULT NULL,
  `locZ` float DEFAULT NULL,
  `instanceName` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `id` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `teleport_effects`
--

LOCK TABLES `teleport_effects` WRITE;
/*!40000 ALTER TABLE `teleport_effects` DISABLE KEYS */;
/*!40000 ALTER TABLE `teleport_effects` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `username` varchar(32) NOT NULL,
  `password` varchar(32) NOT NULL,
  PRIMARY KEY (`username`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `voxeland_changes`
--

DROP TABLE IF EXISTS `voxeland_changes`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `voxeland_changes` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `voxelandid` int(11) NOT NULL,
  `x` int(11) NOT NULL,
  `y` int(11) NOT NULL,
  `z` int(11) NOT NULL,
  `type` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `voxeland_changes`
--

LOCK TABLES `voxeland_changes` WRITE;
/*!40000 ALTER TABLE `voxeland_changes` DISABLE KEYS */;
/*!40000 ALTER TABLE `voxeland_changes` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `voxelands`
--

DROP TABLE IF EXISTS `voxelands`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `voxelands` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `instance` varchar(45) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

CREATE TABLE `editor_option` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `optionType` VARCHAR(45) NOT NULL,
  `deletable` TINYINT(1) NULL DEFAULT 1,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `optionType_UNIQUE` (`optionType` ASC));

INSERT INTO `editor_option` VALUES (1,'Item Type',1),(2,'Weapon Type',1),
(3,'Armor Type',1),(4,'Species',1),(5,'Race',1),(6,'Class',1),
(7,'Crafting Station',1),(8,'Dialogue Action',1),(9,'Mob Type',1),
(10,'Stat Functions',1),(11,'Target Type',1),(12,'Item Effect Type',1),
(13,'Quest Objective Type',1),(14,'Stat Shift Requirement',0),(15,'Stat Shift Action',0);

CREATE TABLE `editor_option_choice` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `optionTypeID` INT NOT NULL,
  `choice` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`));
  
INSERT INTO `editor_option_choice` VALUES (1,1,'Weapon'),(2,1,'Armor'),(3,1,'Consumable'),
(4,1,'Material'),(5,2,'Sword'),(6,2,'Axe'),(7,2,'Mace'),(8,2,'Staff'),(9,2,'Bow'),
(10,2,'Gun'),(11,3,'Cloth'),(12,3,'Leather'),(13,3,'Mail'),(14,3,'Plate'),(15,1,'Junk'),
(16,4,'Humanoid'),(17,4,'Beast'),(18,4,'Dragon'),(19,4,'Elemental'),(20,4,'Undead'),
(22,5,'Human'),(23,6,'Warrior'),(24,6,'Mage'),(25,6,'Rogue'),(26,7,'Anvil'),(27,7,'Smelter'),
(28,7,'Pot'),(29,7,'Oven'),(30,7,'Cauldron'),(31,7,'Sawmill'),(32,7,'Loom'),(33,7,'Sewing Table'),
(34,7,'Tannery'),(35,7,'Masonry Table'),(36,8,'Dialogue'),(37,8,'Quest'),(38,8,'Ability'),
(39,9,'Normal'),(40,9,'Untargetable'),(41,9,'Boss'),(42,9,'Rare'),(43,10,'Health Mod'),
(44,10,'Mana Mod'),(45,10,'Physical Power'),(46,10,'Magical Power'),(47,10,'Physical Accuracy'),
(48,10,'Magical Accuracy'),(49,11,'Enemy'),(50,11,'Self'),(51,11,'Friendly'),
(52,11,'Friend Not Self'),(53,11,'Group'),(54,11,'AoE Enemy'),(55,11,'AoE Friendly'),
(56,1,'Quest'),(57,12,'Stat'),(58,12,'UseAbility'),(59,12,'AutoAttack'),(60,13,'item'),
(61,13,'mob'),(62,1,'Bag'),(63,1,'Container'),(64,12,'ClaimObject'),(65,12,'CreateClaim'),
(66,12,'StartQuest'),(67,14,'combatstate'),(68,14,'deadstate'),(69,15,'death'),
(70,10,'Health'),(71,10,'Mana'),(72,12,'Currency');
  
CREATE TABLE `game_setting` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NOT NULL,
  `datatype` VARCHAR(45) NOT NULL,
  `value` VARCHAR(45) NOT NULL,
  PRIMARY KEY (`id`));
  
INSERT INTO `game_setting` (`name`, `datatype`, `value`) VALUES ('PLAYER_BAG_COUNT', 'int', '4'),
('PLAYER_DEFAULT_BAG_SIZE', 'int', '16'),
('MOB_DEATH_EXP', 'bool', 'true');
  
  
CREATE TABLE `resource_drop` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `resource_template` INT NULL,
  `item` INT NULL,
  `min` INT NULL,
  `max` INT NULL,
  `chance` FLOAT NULL,
  PRIMARY KEY (`id`));

CREATE TABLE `resource_node_template` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(45) NULL,
  `skill` INT NULL,
  `skillLevel` INT NULL,
  `skillLevelMax` INT NULL,
  `weaponReq` VARCHAR(45) NULL,
  `equipped` TINYINT(1) NULL,
  `gameObject` VARCHAR(128) NULL,
  `coordEffect` VARCHAR(128) NULL,
  `instance` VARCHAR(45) NULL,
  `respawnTime` INT NULL,
  `locX` FLOAT NULL,
  `locY` FLOAT NULL,
  `locZ` FLOAT NULL,
  `harvestCount` INT NULL,
  `harvestTimeReq` FLOAT NULL DEFAULT 0,
  PRIMARY KEY (`id`));
  
CREATE TABLE `resource_node_spawn` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `instance` VARCHAR(45) NULL,
  `resourceTemplate` INT NULL,
  `respawnTime` INT NULL,
  `locX` FLOAT NULL,
  `locY` FLOAT NULL,
  `locZ` FLOAT NULL,
  PRIMARY KEY (`id`));