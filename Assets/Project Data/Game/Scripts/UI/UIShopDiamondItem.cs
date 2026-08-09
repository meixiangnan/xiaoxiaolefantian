using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIShopDiamondItem : MonoBehaviour
    {
        [SerializeField] Button buyBtn;
        [SerializeField] int diamondAmount;
        [SerializeField] int priceYuan;

        private System.Action onPurchaseSuccess;

        public void Init(System.Action onPurchased)
        {
            onPurchaseSuccess = onPurchased;
            buyBtn.onClick.AddListener(OnBuyClick);
        }

        private void OnBuyClick()
        {
            NotifyDialog.NotifyClose(DialogState.NoticeConfirmOnly, "提示", $"是否消费{priceYuan}元购买{diamondAmount}钻石？", HandleConfirmedPurchase, TextAlignmentOptions.Center);
        }

        private void HandleConfirmedPurchase()
        {
            var roleModule = GameGlobal.Instance.GetModule<RoleModule>();
            if (roleModule == null || roleModule.IsAdult()) 
            {
                GrantDiamond();
                return;
            }

            if (!TryPassMinorRechargeLimit(roleModule.Age))
            {
                return;
            }

            var diamondModule = GameGlobal.Instance.GetModule<DiamondModule>();
            diamondModule.RecordRecharge(priceYuan);
            GrantDiamond();
        }

        private bool TryPassMinorRechargeLimit(int age)
        {
            if (age < 8)
            {
                NotifyDialog.NotifyClose(DialogState.NoticeConfirmOnly, "防沉迷提示", "       根据《关于防止未成年人沉迷网络游戏的通知》，本游戏不为未满8周岁的用户提供游戏充值服务。");
                return false;
            }

            var diamondModule = GameGlobal.Instance.GetModule<DiamondModule>();
            int monthlyAmountAfterPurchase = diamondModule.MonthlyRechargeAmount + priceYuan;

            if (age < 16)
            {
                if (priceYuan > 50)
                {
                    NotifyDialog.NotifyClose(DialogState.NoticeConfirmOnly, "防沉迷提示", "       根据《关于防止未成年人沉迷网络游戏的通知》，游戏中8周岁以上未满16周岁的用户，单笔充值金额不得超过50元人民币。您已超出支付上限，无法继续充值。");
                    return false;
                }

                if (monthlyAmountAfterPurchase > 200)
                {
                    NotifyDialog.NotifyClose(DialogState.NoticeConfirmOnly, "防沉迷提示", "       根据《关于防止未成年人沉迷网络游戏的通知》，游戏中8周岁以上未满16周岁的用户，每月充值金额累计不得超过200元人民币。您已超出支付上限，无法继续充值。");
                    return false;
                }

                return true;
            }

            if (priceYuan > 100)
            {
                NotifyDialog.NotifyClose(DialogState.NoticeConfirmOnly, "防沉迷提示", "        根据《关于防止未成年人沉迷网络游戏的通知》，游戏中16周岁以上未满18周岁的用户，单笔充值金额不得超过100元人民币。您已超出支付上限，无法继续充值。");
                return false;
            }

            if (monthlyAmountAfterPurchase > 400)
            {
                NotifyDialog.NotifyClose(DialogState.NoticeConfirmOnly, "防沉迷提示", "        根据《关于防止未成年人沉迷网络游戏的通知》，游戏中16周岁以上未满18周岁的用户，每月充值金额累计不得超过400元人民币。您已超出支付上限，无法继续充值。");
                return false;
            }

            return true;
        }

        private void GrantDiamond()
        {
            var diamondModule = GameGlobal.Instance.GetModule<DiamondModule>();
            diamondModule.AddDiamond(diamondAmount);
            FloatingMessage.ShowMessage($"+{diamondAmount} 钻石。");
            onPurchaseSuccess?.Invoke();
        }
    }
}
