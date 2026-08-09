using System;
using UnityEngine;
using Watermelon.GameModule;

namespace Watermelon
{
    public class DiamondModule : GameModuleBase
    {
        private int diamondCount;
        private string monthlyRechargeMonth = "";
        private int monthlyRechargeAmount;

        public int DiamondCount => diamondCount;
        public int MonthlyRechargeAmount
        {
            get
            {
                RefreshRechargeMonth();
                return monthlyRechargeAmount;
            }
        }

        public event System.Action OnDiamondChanged;

        public override void Init(GameModuleManager mngr)
        {
            base.Init(mngr);
            LoadDiamond();
        }

        private void LoadDiamond()
        {
            diamondCount = 0;
            RefreshRechargeMonth(false);
        }

        public void SetServerData(int serverDiamondCount, string serverMonthlyRechargeMonth, int serverMonthlyRechargeAmount)
        {
            diamondCount = Math.Max(0, serverDiamondCount);
            monthlyRechargeMonth = serverMonthlyRechargeMonth ?? "";
            monthlyRechargeAmount = Math.Max(0, serverMonthlyRechargeAmount);
            RefreshRechargeMonth(false);
            OnDiamondChanged?.Invoke();
        }

        public string GetMonthlyRechargeMonth()
        {
            RefreshRechargeMonth(false);
            return monthlyRechargeMonth;
        }

        private void RefreshRechargeMonth(bool upload = true)
        {
            string currentMonth = DateTime.Now.ToString("yyyyMM");
            if (monthlyRechargeMonth == currentMonth)
            {
                return;
            }

            monthlyRechargeMonth = currentMonth;
            monthlyRechargeAmount = 0;
            if (upload)
            {
                GameGlobal.Instance?.UploadRoleData();
            }
        }

        public void RecordRecharge(int yuanAmount)
        {
            RefreshRechargeMonth(false);
            monthlyRechargeAmount += yuanAmount;
            GameGlobal.Instance?.UploadRoleData();
        }

        private void SaveDiamond()
        {
            GameGlobal.Instance?.UploadRoleData();
        }

        public void AddDiamond(int amount)
        {
            diamondCount += amount;
            SaveDiamond();
            OnDiamondChanged?.Invoke();
        }

        public bool SpendDiamond(int amount)
        {
            if (diamondCount >= amount)
            {
                diamondCount -= amount;
                SaveDiamond();
                OnDiamondChanged?.Invoke();
                return true;
            }
            return false;
        }
    }
}
