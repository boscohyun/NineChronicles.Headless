using Bencodex.Types;
using GraphQL;
using GraphQL.Types;
using Libplanet;
using Libplanet.Action;
using Libplanet.Assets;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Nekoyume.Action;
using Nekoyume.Model.State;
using Serilog;
using System;
using System.Collections.Generic;
using Libplanet.Tx;
using NineChroniclesActionType = Libplanet.Action.PolymorphicAction<Nekoyume.Action.ActionBase>;

namespace NineChronicles.Headless.GraphTypes
{
    public class ActionMutation : ObjectGraphType<NineChroniclesNodeService>
    {
        public ActionMutation()
        {
            Field<NonNullGraphType<TxIdType>>("createAvatar",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>>
                    {
                        Name = "avatarName",
                        Description = "The character name."
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "avatarIndex",
                        Description = "The index of character slot. 0 ~ 2"
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "hairIndex",
                        Description = "The index of character hair color. 0 ~ 8"
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "lensIndex",
                        Description = "The index of character eye color. 0 ~ 8"
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "earIndex",
                        Description = "The index of character ear color. 0 ~ 8"
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "tailIndex",
                        Description = "The index of character tail color. 0 ~ 8"
                    }
                ),
                resolve: context =>
                {
                    try
                    {
                        NineChroniclesNodeService service = context.Source;
                        PrivateKey privateKey = service.PrivateKey;
                        BlockChain<NineChroniclesActionType> blockChain = service.Swarm.BlockChain;
                        var avatarName = context.GetArgument<string>("avatarName");
                        var avatarIndex = context.GetArgument<int>("avatarIndex");
                        var hairIndex = context.GetArgument<int>("hairIndex");
                        var lensIndex = context.GetArgument<int>("lensIndex");
                        var earIndex = context.GetArgument<int>("earIndex");
                        var tailIndex = context.GetArgument<int>("tailIndex");
                        var action = new CreateAvatar2
                        {
                            index = avatarIndex,
                            hair = hairIndex,
                            lens = lensIndex,
                            ear = earIndex,
                            tail = tailIndex,
                            name = avatarName,
                        };

                        var actions = new PolymorphicAction<ActionBase>[] { action };
                        Transaction<PolymorphicAction<ActionBase>> tx = blockChain.MakeTransaction(privateKey, actions);
                        return tx.Id;
                    }
                    catch (Exception e)
                    {
                        var msg = $"Unexpected exception occurred during {typeof(ActionMutation)}: {e}";
                        context.Errors.Add(new ExecutionError(msg, e));
                        Log.Error(msg, e);
                        throw;
                    }
                });

            Field<NonNullGraphType<TxIdType>>("hackAndSlash",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "avatarAddress",
                        Description = "AvatarState address."
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "worldId",
                        Description = "World ID."
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "stageId",
                        Description = "Stage ID."
                    },
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "weeklyArenaAddress",
                        Description = "Address of this WeeklyArenaState"
                    },
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "rankingArenaAddress",
                        Description = "AvatarState rankingMapAddress."
                    },
                    new QueryArgument<ListGraphType<GuidGraphType>>
                    {
                        Name = "costumeIds",
                        Description = "List of costume id for equip."
                    },
                    new QueryArgument<ListGraphType<GuidGraphType>>
                    {
                        Name = "equipmentIds",
                        Description = "List of equipment id for equip."
                    },
                    new QueryArgument<ListGraphType<GuidGraphType>>
                    {
                        Name = "consumableIds",
                        Description = "List of consumable id for use."
                    }
                ),
                resolve: context =>
                {
                    try
                    {
                        NineChroniclesNodeService service = context.Source;
                        BlockChain<NineChroniclesActionType> blockChain = service.Swarm.BlockChain;
                        Address weeklyArenaAddress = context.GetArgument<Address>("weeklyArenaAddress");
                        Address rankingArenaAddress = context.GetArgument<Address>("rankingArenaAddress");
                        Address avatarAddress = context.GetArgument<Address>("avatarAddress");
                        int worldId = context.GetArgument<int>("worldId");
                        int stageId = context.GetArgument<int>("stageId");
                        List<Guid> costumeIds = context.GetArgument<List<Guid>>("costumeIds") ?? new List<Guid>();
                        List<Guid> equipmentIds = context.GetArgument<List<Guid>>("equipmentIds") ?? new List<Guid>();
                        List<Guid> consumableIds = context.GetArgument<List<Guid>>("consumableIds") ?? new List<Guid>();

                        var action = new HackAndSlash4
                        {
                            avatarAddress = avatarAddress,
                            worldId = worldId,
                            stageId = stageId,
                            WeeklyArenaAddress = weeklyArenaAddress,
                            RankingMapAddress = rankingArenaAddress,
                            costumes = costumeIds,
                            equipments = equipmentIds,
                            foods = consumableIds,
                        };

                        var actions = new PolymorphicAction<ActionBase>[] { action };
                        Transaction<PolymorphicAction<ActionBase>> tx = blockChain.MakeTransaction(context.Source.PrivateKey, actions);
                        return tx.Id;
                    }
                    catch (Exception e)
                    {
                        var msg = $"Unexpected exception occurred during {typeof(ActionMutation)}: {e}";
                        context.Errors.Add(new ExecutionError(msg, e));
                        Log.Error(msg, e);
                        throw;
                    }
                });

            Field<NonNullGraphType<TxIdType>>("combinationEquipment",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "avatarAddress",
                        Description = "AvatarState address."
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "recipeId",
                        Description = "EquipmentRecipe ID."
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "slotIndex",
                        Description =  "The index of combination slot. 0 ~ 3"
                    },
                    new QueryArgument<IntGraphType>
                    {
                        Name = "subRecipeId",
                        Description = "EquipmentSubRecipe ID."
                    }
                ),
                resolve: context =>
                {
                    try
                    {
                        NineChroniclesNodeService service = context.Source;
                        BlockChain<NineChroniclesActionType> blockChain = service.Swarm.BlockChain;
                        int recipeId = context.GetArgument<int>("recipeId");
                        int slotIndex = context.GetArgument<int>("slotIndex");
                        int? subRecipeId = context.GetArgument<int?>("subRecipeId");
                        Address avatarAddress = context.GetArgument<Address>("avatarAddress");

                        var action = new CombinationEquipment4
                        {
                            AvatarAddress = avatarAddress,
                            RecipeId = recipeId,
                            SlotIndex = slotIndex,
                            SubRecipeId = subRecipeId
                        };

                        var actions = new PolymorphicAction<ActionBase>[] { action };
                        Transaction<PolymorphicAction<ActionBase>> tx = blockChain.MakeTransaction(context.Source.PrivateKey, actions);
                        return tx.Id;
                    }
                    catch (Exception e)
                    {
                        var msg = $"Unexpected exception occurred during {typeof(ActionMutation)}: {e}";
                        context.Errors.Add(new ExecutionError(msg, e));
                        Log.Error(msg, e);
                        throw;
                    }
                });

            Field<NonNullGraphType<TxIdType>>("itemEnhancement",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "avatarAddress",
                        Description = "AvatarState address."
                    },
                    new QueryArgument<NonNullGraphType<GuidGraphType>>
                    {
                        Name = "itemId",
                        Description = "Equipment Guid."
                    },
                    new QueryArgument<NonNullGraphType<GuidGraphType>>
                    {
                        Name = "materialId",
                        Description = "Material Guid."
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "slotIndex",
                        Description =  "The index of combination slot. 0 ~ 3"
                    }
                ),
                resolve: context =>
                {
                    try
                    {
                        NineChroniclesNodeService service = context.Source;
                        PrivateKey privatekey = service.PrivateKey;
                        BlockChain<NineChroniclesActionType> blockChain = service.Swarm.BlockChain;
                        Guid itemId = context.GetArgument<Guid>("itemId");
                        Guid materialId = context.GetArgument<Guid>("materialId");
                        Address avatarAddress = context.GetArgument<Address>("avatarAddress");
                        int slotIndex = context.GetArgument<int>("slotIndex");

                        var action = new ItemEnhancement5
                        {
                            avatarAddress = avatarAddress,
                            slotIndex = slotIndex,
                            itemId = itemId,
                            materialId = materialId,
                        };

                        var actions = new PolymorphicAction<ActionBase>[] { action };
                        Transaction<PolymorphicAction<ActionBase>> tx = blockChain.MakeTransaction(privatekey, actions);
                        return tx.Id;
                    }
                    catch (Exception e)
                    {
                        var msg = $"Unexpected exception occurred during {typeof(ActionMutation)}: {e}";
                        context.Errors.Add(new ExecutionError(msg, e));
                        Log.Error(msg, e);
                        throw;
                    }
                });

            Field<NonNullGraphType<TxIdType>>("buy",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "sellerAgentAddress",
                        Description = "ShopItem SellerAgentAddress."
                    },
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "sellerAvatarAddress",
                        Description = "ShopItem SellerAvatarAddress."
                    },
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "buyerAvatarAddress",
                        Description = "AvatarState address."
                    },
                    new QueryArgument<NonNullGraphType<GuidGraphType>>
                    {
                        Name = "productId",
                        Description = "ShopItem Guid."
                    }),
                resolve: context =>
                {
                    try
                    {
                        NineChroniclesNodeService service = context.Source;
                        PrivateKey privateKey = service.PrivateKey;
                        BlockChain<NineChroniclesActionType> blockChain = service.Swarm.BlockChain;
                        Address buyerAvatarAddress = context.GetArgument<Address>("buyerAvatarAddress");
                        Address sellerAgentAddress = context.GetArgument<Address>("sellerAgentAddress");
                        Address sellerAvatarAddress = context.GetArgument<Address>("sellerAvatarAddress");
                        Guid productId = context.GetArgument<Guid>("productId");

                        var action = new Buy4
                        {
                            buyerAvatarAddress = buyerAvatarAddress,
                            sellerAgentAddress = sellerAgentAddress,
                            sellerAvatarAddress = sellerAvatarAddress,
                            productId = productId,
                        };

                        var actions = new PolymorphicAction<ActionBase>[] { action };
                        Transaction<PolymorphicAction<ActionBase>> tx = blockChain.MakeTransaction(privateKey, actions);
                        return tx.Id;
                    }
                    catch (Exception e)
                    {
                        var msg = $"Unexpected exception occurred during {typeof(ActionMutation)}: {e}";
                        context.Errors.Add(new ExecutionError(msg, e));
                        Log.Error(msg, e);
                        throw;
                    }
                });
            Field<NonNullGraphType<TxIdType>>("sell",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "sellerAvatarAddress",
                        Description = "AvatarState address."
                    },
                    new QueryArgument<NonNullGraphType<GuidGraphType>>
                    {
                        Name = "itemId",
                        Description = "Item Guid."
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "price",
                        Description = "Item price."
                    }),
                resolve: context =>
                {
                    try
                    {
                        NineChroniclesNodeService service = context.Source;
                        PrivateKey privateKey = service.PrivateKey;
                        BlockChain<NineChroniclesActionType> blockChain = service.Swarm.BlockChain;
                        Address sellerAvatarAddress = context.GetArgument<Address>("sellerAvatarAddress");
                        Guid itemId = context.GetArgument<Guid>("itemId");
                        var currency = new GoldCurrencyState(
                            (Dictionary)blockChain.GetState(GoldCurrencyState.Address)
                        ).Currency;
                        FungibleAssetValue price = currency * context.GetArgument<int>("price");

                        var action = new Sell3
                        {
                            sellerAvatarAddress = sellerAvatarAddress,
                            itemId = itemId,
                            price = price
                        };

                        var actions = new PolymorphicAction<ActionBase>[] { action };
                        Transaction<PolymorphicAction<ActionBase>> tx = blockChain.MakeTransaction(privateKey, actions);
                        return tx.Id;
                    }
                    catch (Exception e)
                    {
                        var msg = $"Unexpected exception occurred during {typeof(ActionMutation)}: {e}";
                        context.Errors.Add(new ExecutionError(msg, e));
                        throw;
                    }
                });

            Field<NonNullGraphType<TxIdType>>("dailyReward",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "avatarAddress",
                        Description = "AvatarState address."
                    }
                ),
                resolve: context =>
                {
                    try
                    {
                        NineChroniclesNodeService service = context.Source;
                        PrivateKey privateKey = service.PrivateKey;
                        BlockChain<NineChroniclesActionType> blockChain = service.Swarm.BlockChain;
                        Address avatarAddress = context.GetArgument<Address>("avatarAddress");

                        var action = new DailyReward
                        {
                            avatarAddress = avatarAddress
                        };

                        var actions = new PolymorphicAction<ActionBase>[] { action };
                        Transaction<PolymorphicAction<ActionBase>> tx = blockChain.MakeTransaction(privateKey, actions);
                        return tx.Id;
                    }
                    catch (Exception e)
                    {
                        var msg = $"Unexpected exception occurred during {typeof(ActionMutation)}: {e}";
                        context.Errors.Add(new ExecutionError(msg, e));
                        Log.Error(msg, e);
                        throw;
                    }
                });

            Field<NonNullGraphType<TxIdType>>("combinationConsumable",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<AddressType>>
                    {
                        Name = "avatarAddress",
                        Description = "AvatarState address."
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "recipeId",
                        Description = "ConsumableRecipe ID."
                    },
                    new QueryArgument<NonNullGraphType<IntGraphType>>
                    {
                        Name = "slotIndex",
                        Description =  "The index of combination slot. 0 ~ 3"
                    }
                ),
                resolve: context =>
                {
                    try
                    {
                        NineChroniclesNodeService service = context.Source;
                        BlockChain<NineChroniclesActionType> blockChain = service.Swarm.BlockChain;
                        int recipeId = context.GetArgument<int>("recipeId");
                        int slotIndex = context.GetArgument<int>("slotIndex");
                        Address avatarAddress = context.GetArgument<Address>("avatarAddress");

                        var action = new CombinationConsumable3
                        {
                            AvatarAddress = avatarAddress,
                            recipeId = recipeId,
                            slotIndex = slotIndex,
                        };

                        var actions = new PolymorphicAction<ActionBase>[] { action };
                        Transaction<PolymorphicAction<ActionBase>> tx = blockChain.MakeTransaction(context.Source.PrivateKey, actions);
                        return tx.Id;
                    }
                    catch (Exception e)
                    {
                        var msg = $"Unexpected exception occurred during {typeof(ActionMutation)}: {e}";
                        context.Errors.Add(new ExecutionError(msg, e));
                        throw;
                    }
                }
            );
        }
    }
}
