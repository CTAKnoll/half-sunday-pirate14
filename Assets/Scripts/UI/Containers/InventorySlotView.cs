using TMPro;

namespace UI.Containers
{
    public class InventorySlotView : UIView<InventorySlotModel>
    {
        public TextMeshProUGUI StackNumber;
        public override void UpdateViewWithModel(InventorySlotModel model)
        {
            StackNumber.gameObject.SetActive(model.AllowStacking && model.Stacks > 0);
            StackNumber.text = "x" + model.Stacks;
        }
    }
}