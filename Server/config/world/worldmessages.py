from java.util import *
from java.lang import *
from atavism.msgsys import *

#
# This python file creates a world-specific message catalog, and 
# contains definitions for world-specific message  types, if 
# your world makes use of them.  Not all worlds actually
# need to define their own message types, but if if your world does
# need world-specific message types, they must be added to your 
# world-specific message catalog by listing them in this file
#

#
# Create the world message catalog.  Atavism reserves message numbers
# from 1 through 500; the world-specific catalog defined below allocates
# message type numbers from the range 501-1000.
#
worldMessageCatalog = MessageCatalog.addMsgCatalog("worldMessageCatalog", 501, 500);

#
# Add your world-specific messages here.  Each call to addMsgTypeTranslation
# adds the message type which is the second argument to the world message
# catalog.  Each message type must be defined in YourWorldModule by a call
# to MessageType.intern(message_type_string);
# 
#MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, YourWorldModule.MSG_TYPE_YOUR_MESSAGE_TYPE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_ITEM_ACQUIRE_STATUS_CHANGE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_ITEM_EQUIP_STATUS_CHANGE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_REQ_OPEN_MOB)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_REMOVE_SPECIFIC_ITEM)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_REMOVE_GENERIC_ITEM)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_GET_SPECIFIC_ITEM_DATA)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_GET_GENERIC_ITEM_DATA)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_GENERATE_ITEM)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_PLACE_BAG)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_MOVE_BAG)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_REMOVE_BAG)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_MOVE_ITEM)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_LOOT_ITEM)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_LOOT_ALL)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_GENERATE_LOOT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_GET_LOOT_LIST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_GET_MERCHANT_LIST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_PURCHASE_ITEM_FROM_MERCHANT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_PURCHASE_ITEM)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_SELL_ITEM)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_PICKUP_ITEM)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_QUEST_ITEMS_LIST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_SEND_INV_UPDATE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_GET_MAIL)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_MAIL_READ)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_MAIL_TAKE_ITEM)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_RETURN_MAIL)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_DELETE_MAIL)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_SEND_MAIL)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_SEND_PURCHASE_MAIL)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_CHECK_CURRENCY)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_ALTER_CURRENCY)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_GET_SKINS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_RELOAD_ITEMS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_SET_WEAPON)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_PURCHASE_SKIN)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_SET_SKIN_COLOUR)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_GET_ACCOUNT_ITEM_COUNT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_ALTER_ITEM_COUNT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_USE_ACCOUNT_ITEM)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_ITEM_ACTIVATED)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisInventoryClient.MSG_TYPE_RETURNBOOLEAN_CHECK_COMPONENTS)

# Class Ability Client
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ClassAbilityClient.MSG_TYPE_LEVEL_CHANGE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ClassAbilityClient.MSG_TYPE_COMBAT_SKILL_INCREASE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ClassAbilityClient.MSG_TYPE_COMBAT_SKILL_DECREASE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ClassAbilityClient.MSG_TYPE_COMBAT_SKILL_RESET)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ClassAbilityClient.MSG_TYPE_COMBAT_SKILL_ALTER_CURRENT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ClassAbilityClient.MSG_TYPE_COMBAT_GET_SKILL)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ClassAbilityClient.MSG_TYPE_COMBAT_GET_PLAYER_SKILL_LEVEL)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ClassAbilityClient.MSG_TYPE_LEARN_ABILITY)
# Quest Client
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, QuestClient.MSG_TYPE_START_QUEST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, QuestClient.MSG_TYPE_QUEST_ITEM_REQS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, QuestClient.MSG_TYPE_QUEST_ITEM_UPDATE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, QuestClient.MSG_TYPE_QUEST_TASK_UPDATE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, QuestClient.MSG_TYPE_REQ_QUEST_PROGRESS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, QuestClient.MSG_TYPE_COMPLETE_QUEST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, QuestClient.MSG_TYPE_QUEST_CONCLUDE_UPDATE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, QuestClient.MSG_TYPE_ABANDON_QUEST)
# Combat Client
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_COMBAT_MOB_DEATH)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_COMBAT_ABILITY_USED)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_COMBAT_STOP_AUTO_ATTACK)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_TARGET_TYPE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_INTERRUPT_ABILITY)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_COMBAT_LOGOUT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_FACTION_UPDATE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_GET_AOE_TARGETS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_UPDATE_ACTIONBAR)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_APPLY_EFFECT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_ALTER_EXP)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_UPDATE_BREATH)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_UPDATE_FATIGUE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_ALTER_HEARTS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_KNOCKED_OUT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_UPDATE_HEALTH_PROPS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_REGEN_HEALTH_MANA)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, CombatClient.MSG_TYPE_DECREMENT_WEAPON_USES)
# Arena Client
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_GET_ARENA_STATS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_GET_ARENA_TYPES)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_JOIN_QUEUE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_LEAVE_QUEUE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_REMOVE_PLAYER)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_ARENA_KILL)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_PICKUP_FLAG)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DROP_FLAG)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_ACTIVATE_MACHINE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DOT_SCORE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_START_ARENA_CHECK)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_ADD_CREATURE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DESPAWN_GATES)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_END_ARENA)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DUEL_CHALLENGE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DUEL_ACCEPT_CHALLENGE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DUEL_DECLINE_CHALLENGE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DUEL_CHALLENGE_DISCONNECT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DUEL_CHALLENGE_REMOVE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DUEL_START)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DUEL_DEFEAT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DUEL_DISCONNECT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_DUEL_REMOVE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_REMOVE_EFFECTS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_ACTIVATE_ARENA_ABILITY)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_COMPLETE_TUTORIAL)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_CHANGE_RACE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_SELECT_RACE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, ArenaClient.MSG_TYPE_ALTER_EXP)
# Group Client
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, GroupClient.MSG_TYPE_GROUP_INVITE_BY_NAME)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, GroupClient.MSG_TYPE_GET_PLAYER_BY_NAME)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, GroupClient.MSG_TYPE_CREATE_GROUP)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, GroupClient.MSG_TYPE_GET_FRIENDS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, GroupClient.MSG_TYPE_ADD_FRIEND)
# Agis Mob Client
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_GET_INSTANCE_TEMPLATE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_SPAWN_INSTANCE_MOBS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_SPAWN_MOB)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_SPAWN_ARENA_CREATURE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_SPAWN_PET)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_TAME_BEAST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_PET_COMMAND_UPDATE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_SEND_PET_COMMAND)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_UPDATE_PET_STATS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_PET_TARGET_LOST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_TARGET_IN_REACTION_RANGE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_ADD_TARGET_TO_CHECK)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_REMOVE_TARGET_TO_CHECK)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_GET_TEMPLATES)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_CREATE_MOB_SPAWN)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_CREATE_QUEST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_CREATE_LOOT_TABLE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_EDIT_QUEST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_GET_ISLANDS_DATA)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_VERIFY_ISLAND_ACCESS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_ENTER_WORLD)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_REQUEST_DEVELOPER_ACCESS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_CREATE_ISLAND)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_VIEW_MARKERS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_REQUEST_SPAWN_DATA)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_EDIT_SPAWN_MARKER)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_DELETE_SPAWN_MARKER)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_CREATE_MOB)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_EDIT_MOB)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_CREATE_FACTION)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_EDIT_FACTION)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_CATEGORY_UPDATED)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_SPAWN_DOME_MOB)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_DOME_ENQUIRY)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_DOME_ENTRY_REQUEST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_DOME_LEAVE_REQUEST)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_ACTIVATE_DOME_ABILITY)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_MOB_KILLED)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_OBJECT_ACTIVATED)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_GET_INTERACTION_OPTIONS)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_START_INTERACTION)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_DIALOGUE_OPTION_CHOSEN)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_START_DIALOGUE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_PLAY_COORD_EFFECT)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_SET_MOVEMENT_STATE)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_SET_UNDERWATER)
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, AgisMobClient.MSG_TYPE_CHANGE_INSTANCE)

# Faction Client
MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, FactionClient.MSG_TYPE_REPUTATION_UPDATED)

# Social Client
#MessageCatalog.addMsgTypeTranslation(worldMessageCatalog, SocialClient.MSG_TYPE_CHANNEL_CHANGE)

# Data Logger
#MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, DataLoggerClient.MSG_TYPE_DATA_LOG)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, DataLoggerClient.MSG_TYPE_CHARACTER_CREATED)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, DataLoggerClient.MSG_TYPE_CHARACTER_DELETED)
#MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, DataLoggerClient.MSG_TYPE_EVENT_LOG)

# Crafting Client
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, CraftingClient.MSG_TYPE_HARVEST_RESOURCE)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, CraftingClient.MSG_TYPE_GATHER_RESOURCE)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, CraftingClient.MSG_TYPE_CRAFTING_CRAFT_ITEM)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, CraftingClient.MSG_TYPE_CRAFTING_GRID_UPDATED)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, CraftingClient.MSG_TYPE_GET_BLUEPRINTS)

# Voxel Client
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, VoxelClient.MSG_TYPE_CREATE_CLAIM)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, VoxelClient.MSG_TYPE_EDIT_CLAIM)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, VoxelClient.MSG_TYPE_PURCHASE_CLAIM)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, VoxelClient.MSG_TYPE_SELL_CLAIM)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, VoxelClient.MSG_TYPE_DELETE_CLAIM)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, VoxelClient.MSG_TYPE_CLAIM_ACTION)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, VoxelClient.MSG_TYPE_PLACE_CLAIM_OBJECT)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, VoxelClient.MSG_TYPE_EDIT_CLAIM_OBJECT)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, VoxelClient.MSG_TYPE_GET_RESOURCES)
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, VoxelClient.MSG_TYPE_NO_BUILD_CLAIM_TRIGGER)

# Faction Client
MessageCatalog.addMsgTypeTranslation(aoMessageCatalog, FactionClient.MSG_TYPE_UPDATE_PVP_STATE)