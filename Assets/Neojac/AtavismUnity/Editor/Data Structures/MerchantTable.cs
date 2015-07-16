using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;

// Structure of a Atavism Skills
/*
/* Table structure for table `skills`
/*

CREATE TABLE `skills` (
  `id` int(11) NOT NULL,
  `name` varchar(64) NOT NULL,
  `aspect` int(11) DEFAULT NULL,
  `oppositeAspect` int(11) DEFAULT NULL,
  `primaryStat` int(11) NOT NULL,
  `secondaryStat` int(11) NOT NULL,
  `thirdStat` int(11) NOT NULL,
  `fourthStat` int(11) NOT NULL,
  `ability1` int(11) DEFAULT NULL,
  `ability1Level` int(11) DEFAULT NULL,
  `ability2` int(11) DEFAULT NULL,
  `ability2Level` int(11) DEFAULT NULL,
  `ability3` int(11) DEFAULT NULL,
  `ability3Level` int(11) DEFAULT NULL,
  `ability4` int(11) DEFAULT NULL,
  `ability4Level` int(11) DEFAULT NULL,
  `ability5` int(11) DEFAULT NULL,
  `ability5Level` int(11) DEFAULT NULL,
  `ability6` int(11) DEFAULT NULL,
  `ability6Level` int(11) DEFAULT NULL,
  `ability7` int(11) DEFAULT NULL,
  `ability7Level` int(11) DEFAULT NULL,
  `ability8` int(11) DEFAULT NULL,
  `ability8Level` int(11) DEFAULT NULL,
  `ability9` int(11) DEFAULT NULL,
  `ability9Level` int(11) DEFAULT NULL,
  `ability10` int(11) DEFAULT NULL,
  `ability10Level` int(11) DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `id` (`id`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

*/

public class MerchantTableItemEntry : DataStructure
{
	public MerchantTableItemEntry() : this(-1, -1, -1, 0) {
	}

	public MerchantTableItemEntry(int count, int itemID) : this(count, itemID, -1, 0) {
	}

	public MerchantTableItemEntry(int count, int itemID, int entryID, int refreshTime) {
		this.id = entryID;
		this.count = count;
		this.itemID = itemID;
		this.refreshTime = refreshTime;

		fields = new Dictionary<string, string> () {
			{"id", "int"},
			{"tableID", "int"},
			{"count", "int"},
			{"itemID", "int"},
			{"refreshTime", "int"},
		};
	}
	
	public int tableID;
	public int count = -1;
	public int itemID;
	public int refreshTime = 0;

	public MerchantTableItemEntry Clone()
	{
		return (MerchantTableItemEntry) this.MemberwiseClone();
	}
	
	public override string GetValue (string fieldKey)
	{
		if (fieldKey == "id") {
			return id.ToString();
		} else if (fieldKey == "tableID") {
			return tableID.ToString();
		} else if (fieldKey == "count") {
			return count.ToString();
		} else if (fieldKey == "itemID") {
			return itemID.ToString();
		} else if (fieldKey == "refreshTime") {
			return refreshTime.ToString();
		}
		return "";
	}
}

public class MerchantTable: DataStructure
{
	public int id = 1;					// Database Index
	// General Parameters
	public string name = "name";		// The skill template name

	public List<MerchantTableItemEntry> tableItems = new List<MerchantTableItemEntry>();
	
	public List<int> itemsToBeDeleted = new List<int>();

	public MerchantTable () 
	{
		// Database fields
		fields = new Dictionary<string, string> () {
			{"name", "string"},
		};

	}
	
	public MerchantTable Clone()
	{
		return (MerchantTable) this.MemberwiseClone();
	}
		
	public override string GetValue (string fieldKey)
	{
		switch (fieldKey) {
		case "id":
			return id.ToString();
			break;
		case "name":
			return name;
			break;
		}
		return "";
	}
		
}