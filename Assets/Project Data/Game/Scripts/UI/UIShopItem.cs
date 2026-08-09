using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class UIShopItem : MonoBehaviour
    {
        [SerializeField] Button buyBtn;
        [SerializeField] PUType itemType;
        [SerializeField] int costDiamond;
        [SerializeField] int itemAmount = 1;

        private System.Action onPurchaseSuccess;

        public void Init(System.Action onPurchased)
        {
            onPurchaseSuccess = onPurchased;
            buyBtn.onClick.AddListener(OnBuyClick);
        }

        private void OnBuyClick()
        {
            var diamondModule = GameGlobal.Instance.GetModule<DiamondModule>();
            if (!diamondModule.SpendDiamond(costDiamond))
            {
                FloatingMessage.ShowMessage("钻石不足。");
                return;
            }

            var roleModule = GameGlobal.Instance.GetModule<RoleModule>();
            roleModule.AddPowerUp(itemType, itemAmount);
            FloatingMessage.ShowMessage($"购买成功 +{itemAmount}。");
            onPurchaseSuccess?.Invoke();
        }
    }
}
