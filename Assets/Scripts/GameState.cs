using System;
using System.Collections.Generic;
using UnityEngine;

namespace MergeMortem
{
    [Serializable]
    public class BoardItem
    {
        public ChainType chain;
        public int tier; // 1..3
        public int cellIndex;

        public BoardItem(ChainType chain, int tier, int cellIndex)
        {
            this.chain = chain;
            this.tier = tier;
            this.cellIndex = cellIndex;
        }
    }

    public enum MoveResultType
    {
        Moved,
        Merged,
        SelectionChanged,
        SelectionCleared,
        NoOp
    }

    public struct MoveResult
    {
        public MoveResultType type;
        public int mergedFromIndex;
        public int mergedIntoIndex;
    }

    public enum SpawnResultType
    {
        Spawned,
        BoardFull,
        ChainFiled,
        NoEnergy
    }

    public class GameState
    {
        public const int CellCount = Data.BoardSize * Data.BoardSize;

        public BoardItem[] board = new BoardItem[CellCount];
        public int energy = Data.StartEnergy;
        public int coins = 0;
        public int xp = 0;
        public int? selectedIndex = null;

        public bool[] chainFiled = new bool[3];

        public int Level
        {
            get { return 1 + xp / Data.XpPerLevel; }
        }

        public int XpIntoLevel
        {
            get { return xp % Data.XpPerLevel; }
        }

        public void Reset()
        {
            for (int i = 0; i < CellCount; i++) board[i] = null;
            energy = Data.StartEnergy;
            coins = 0;
            xp = 0;
            selectedIndex = null;
            chainFiled[0] = chainFiled[1] = chainFiled[2] = false;
        }

        public int FirstEmptyCell()
        {
            for (int i = 0; i < CellCount; i++)
                if (board[i] == null) return i;
            return -1;
        }

        public bool IsBoardFull()
        {
            return FirstEmptyCell() == -1;
        }

        public SpawnResultType SpawnBasic(ChainType chain)
        {
            int chainIdx = (int)chain;
            if (chainFiled[chainIdx]) return SpawnResultType.ChainFiled;
            if (energy <= 0) return SpawnResultType.NoEnergy;

            int cell = FirstEmptyCell();
            if (cell == -1) return SpawnResultType.BoardFull;

            energy = Mathf.Max(0, energy - 1);
            board[cell] = new BoardItem(chain, 1, cell);
            return SpawnResultType.Spawned;
        }

        /// <summary>
        /// Called when the player taps a cell. Implements select/move/merge/change-selection rules.
        /// </summary>
        public MoveResult TapCell(int index)
        {
            MoveResult result = new MoveResult { type = MoveResultType.NoOp, mergedFromIndex = -1, mergedIntoIndex = -1 };

            if (selectedIndex == null)
            {
                if (board[index] != null)
                {
                    selectedIndex = index;
                    result.type = MoveResultType.SelectionChanged;
                }
                return result;
            }

            int selIdx = selectedIndex.Value;

            if (selIdx == index)
            {
                selectedIndex = null;
                result.type = MoveResultType.SelectionCleared;
                return result;
            }

            BoardItem selItem = board[selIdx];
            BoardItem targetItem = board[index];

            if (selItem == null)
            {
                selectedIndex = null;
                if (targetItem != null)
                {
                    selectedIndex = index;
                    result.type = MoveResultType.SelectionChanged;
                }
                return result;
            }

            if (targetItem == null)
            {
                board[index] = selItem;
                selItem.cellIndex = index;
                board[selIdx] = null;
                selectedIndex = null;
                result.type = MoveResultType.Moved;
                return result;
            }

            bool sameChain = targetItem.chain == selItem.chain;
            bool sameTier = targetItem.tier == selItem.tier;
            bool canMerge = sameChain && sameTier && selItem.tier < Data.TierCount;

            if (canMerge)
            {
                targetItem.tier += 1;
                board[selIdx] = null;
                xp += Data.XpPerMerge;
                selectedIndex = null;
                result.type = MoveResultType.Merged;
                result.mergedFromIndex = selIdx;
                result.mergedIntoIndex = index;
                return result;
            }

            selectedIndex = index;
            result.type = MoveResultType.SelectionChanged;
            return result;
        }

        public bool CanFileSelected()
        {
            if (selectedIndex == null) return false;
            BoardItem item = board[selectedIndex.Value];
            if (item == null) return false;
            if (item.tier != Data.TierCount) return false;
            if (chainFiled[(int)item.chain]) return false;
            return true;
        }

        /// <summary>
        /// Files the currently selected top-tier item. Returns the chain filed, or null if nothing filed.
        /// </summary>
        public ChainType? FileSelected()
        {
            if (!CanFileSelected()) return null;
            int idx = selectedIndex.Value;
            BoardItem item = board[idx];
            ChainType chain = item.chain;

            board[idx] = null;
            selectedIndex = null;
            chainFiled[(int)chain] = true;
            coins += Data.CoinsPerFile;
            xp += Data.XpPerFile;
            return chain;
        }

        public bool AllChainsFiled()
        {
            return chainFiled[0] && chainFiled[1] && chainFiled[2];
        }

        public void WatchAdForEnergy()
        {
            energy += Data.AdEnergyReward;
        }

        public bool HasMatchingPairOnBoard()
        {
            for (int i = 0; i < CellCount; i++)
            {
                BoardItem a = board[i];
                if (a == null) continue;
                for (int j = i + 1; j < CellCount; j++)
                {
                    BoardItem b = board[j];
                    if (b == null) continue;
                    if (a.chain == b.chain && a.tier == b.tier && a.tier < Data.TierCount)
                        return true;
                }
            }
            return false;
        }

        public bool HasTopTierUnfiledOnBoard()
        {
            for (int i = 0; i < CellCount; i++)
            {
                BoardItem a = board[i];
                if (a == null) continue;
                if (a.tier == Data.TierCount && !chainFiled[(int)a.chain]) return true;
            }
            return false;
        }

        public bool HasAnyItemOnBoard()
        {
            for (int i = 0; i < CellCount; i++)
                if (board[i] != null) return true;
            return false;
        }
    }
}
