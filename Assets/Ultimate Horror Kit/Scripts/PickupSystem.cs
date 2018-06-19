using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AxlPlay
{
    [System.Serializable]
    public class ItemInInventoryOld
    {
        public GameObject item;
        public InteractiveObject script;
        public Weapon weapon;
        public Image canvasIcon;
    }
    public class PickupSystem : MonoBehaviour
    {
        public bool DELETEALLPREFS;
        public InteractiveObject[] StartItems;
        [HideInInspector]

        public List<InteractiveObject> StartItemsSpawned = new List<InteractiveObject>();


        public int MaxItemsInInventory = 5;
        public List<ItemInInventoryOld> Items = new List<ItemInInventoryOld>();

       // [HideInInspector]
        public ItemInInventoryOld CurrentItem = null;

        public Image[] InventoryItemsIcons;

        public PlayerController playerController;
        public float PickDistance = 3f;
        public UIEffects Default_Crosshair;
        public UIEffects Interact_Crosshair;

        public UIEffects Description;
        public Text ItemNameTxt;
        public Text DescriptionTxt;

        public LayerMask InteractiveObjects;
        //private
        [HideInInspector]
        public GameObject objectLooking;

        [HideInInspector]
        public bool IsExamining;

        private InteractiveObject ItemExaminingScript;

        private InteractiveObject PreviousItem = null;
        private void Awake()
        {
            if (!PlayerPrefs.HasKey("firstPlay"))
                PlayerPrefs.SetString("firstPlay", "true");
            else
                PlayerPrefs.SetString("firstPlay", "false");

        }
        private void Start()
        {
            // add observer to receive a call from a method
            NotificationCenter.DefaultCenter.AddObserver(this, "ItemConsumed");            
        }
        void Update()
        {
            if (DELETEALLPREFS)
            {
                DELETEALLPREFS = false;
                DeleteData();
            }

            Vector3 position = transform.position;
            Vector3 direction = transform.TransformDirection(Vector3.forward);
            // raycast to interact with items
            RaycastHit hit;

            if (Physics.Raycast(position, direction, out hit, PickDistance, InteractiveObjects.value))
            {
                if (objectLooking != hit.transform.gameObject)
                {

                    objectLooking = hit.transform.gameObject;
                    objectLooking.SendMessage("StartLooking", SendMessageOptions.DontRequireReceiver);
                }

                if (hit.transform.GetComponent<InteractiveObject>() != null)
                    Interact(hit.transform.gameObject);
                else
                    Interact(hit.transform.root.gameObject);
                //	target = hit.transform.gameObject;
            }
            else
            {
                if (objectLooking != null)
                {
                    objectLooking.SendMessage("StopLooking", SendMessageOptions.DontRequireReceiver);
                    objectLooking = null;
                }
                Description.DoFadeOut();
                // set crosshairs
                if (Default_Crosshair)
                    Default_Crosshair.DoFadeIn();
                if (Interact_Crosshair)
                    Interact_Crosshair.DoFadeOut();

                if (ItemExaminingScript != null)
                {
                    // examine item or stop examining
                    if (InputManager.inputManager.GetButtonDown(InputManager.inputManager.KeyToExamine))
                    {
                        ToggleExamination(ItemExaminingScript.gameObject);

                    }

                }
            }

        }

        public void ToggleCurrentItem()
        {

            if (CurrentItem != null && CurrentItem.item != null && CurrentItem.script != null)
            {
                PreviousItem = CurrentItem.script;
           

                StopUsing();
                return;
            }
            else if(PreviousItem != null )
            {

                UseItem(PreviousItem);
            }
           /* if(CurrentItem.item != null)
            Debug.Log("current " + CurrentItem.item.name); //+ " " + CurrentItem.item + " " + CurrentItem.script);
            Debug.Log("previous " + PreviousItem.name); // + " " + PreviousItem.item + " " + PreviousItem.script);
            */
        }
        // example: when click looking an object
        private void Interact(GameObject target)
        {
            Default_Crosshair.DoFadeOut();
            Interact_Crosshair.DoFadeIn();

            InteractiveObject interactiveObject = target.transform.gameObject.GetComponent<InteractiveObject>();
            if (interactiveObject != null)
            {
                if (interactiveObject.Type == InteractiveObject.Types.Usable || interactiveObject.Type == InteractiveObject.Types.Consumable)
                {

                    if (InputManager.inputManager.GetButtonDown(interactiveObject.InputToInteract))
                    {
                        if (!IsExamining)
                        {
                            if (Items.Count < MaxItemsInInventory)
                            {
                                // pick up the item and add it into inventory
                                PickupItem(interactiveObject);
                            }
                            else
                            {

                            }
                        }
                    }
                }
                // send messages and call methods of the item script
                if (interactiveObject.Type == InteractiveObject.Types.OneClickInteraction)
                {

                    if (InputManager.inputManager.GetButtonDown(interactiveObject.InputToInteract))
                    {

                        target.SendMessage("Interaction", target);
                    }
                }
                if (interactiveObject.Type == InteractiveObject.Types.Examinable || interactiveObject.Examinable)
                {
                    if (InputManager.inputManager.GetButtonDown(interactiveObject.InputToExamine))
                    {
                        ToggleExamination(target);

                    }

                }
                // show name and description of the item
                if (interactiveObject.ShowItemSettings)
                {
                    Description.DoFadeIn();

                    DescriptionTxt.text = interactiveObject.Description;
                    ItemNameTxt.text = interactiveObject.ItemName;
                }
            }
        }
        // examinate or stop examining object.
        public void ToggleExamination(GameObject item)
        {
            IsExamining = !IsExamining;

            if (IsExamining)
            {

                ItemExaminingScript = item.GetComponent<InteractiveObject>();
                item.SendMessage("StartExamining");
            }
            else
            {
                ItemExaminingScript = null;
                item.SendMessage("StopExamining");
            }
        }


        public void OneClickInteraction(GameObject item)
        {
            if (item != null)
            {
                item.SendMessage("Interaction", SendMessageOptions.DontRequireReceiver);
            }
        }
       // called from items to know if they are being used by the player
        public bool IsUsing(GameObject _item)
        {
            // when delete data on test button on middle game
            if (!_item)
                return false;
            if (CurrentItem != null && CurrentItem.item == _item)
                return true;
            return false;
        }

        public void PickupItem(InteractiveObject interactiveObject)
        {
            // add to the list
            ItemInInventoryOld newItemInInventory = new ItemInInventoryOld();
            newItemInInventory.item = interactiveObject.gameObject;
            newItemInInventory.script = interactiveObject;
            newItemInInventory.weapon = interactiveObject.GetComponent<Weapon>();

            Image firstEmptyIcon = GetFirstEmptyCanvasItem();
            // put the item icon in the inventory
            if (firstEmptyIcon != null)
            {
                firstEmptyIcon.gameObject.SetActive(true);
                newItemInInventory.canvasIcon = firstEmptyIcon;

                firstEmptyIcon.sprite = interactiveObject.Icon;
                firstEmptyIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(interactiveObject.WidthIcon, interactiveObject.HeightIcon);


                if (!Items.Contains(newItemInInventory))

                    Items.Add(newItemInInventory);


                interactiveObject.SendMessage("Pickuped", playerController);
            }
        }
        // use item (called when click on inventory)
        public void UseItem(Image iconHolder)
        {
            foreach (var item in Items)
            {
                if (item.canvasIcon == iconHolder)
                {
                    if (item.script.Type == InteractiveObject.Types.Usable)
                    {
                        if (CurrentItem != null && CurrentItem.script != null)
                        {
                            CurrentItem.script.SendMessage("StopUsing");
                        }
                        // activate object and start using
                        item.item.gameObject.SetActive(true);
                        item.script.SendMessage("StartUsing", playerController, SendMessageOptions.DontRequireReceiver);
                        CurrentItem = item;

                    }
                    else if (item.script.Type == InteractiveObject.Types.Consumable)
                    {
                        item.script.SendMessage("Consume", SendMessageOptions.DontRequireReceiver);
                        break;
                    }
                }

            }

        }
        // called by the item when consumed
        public void ItemConsumed(NotificationCenter.Notification data)
        {

            if (data != null)
            {
                ItemInInventoryOld deleteItem = null;
                foreach (var item in Items)
                {
                    if (item.item == data.data._gameObject)
                    {
                        item.canvasIcon.sprite = null;
                        item.canvasIcon.gameObject.SetActive(false);
                        deleteItem = item;
                    }
                }
                PlayerPrefs.DeleteKey(deleteItem.script.ItemName);
                PlayerPrefs.Save();
                Items.Remove(deleteItem);

            }
        }
        // use certain item
        public void UseItem(InteractiveObject itemP)
        {
            foreach (var item in Items)
            {

                if (item.script == itemP)
                {
                    if (item.script.Type == InteractiveObject.Types.Usable)
                    {
                        if (CurrentItem != null && CurrentItem.script != null)
                        {
                            CurrentItem.script.SendMessage("StopUsing");
                        }
                        item.item.gameObject.SetActive(true);

                        item.script.SendMessage("StartUsing", playerController, SendMessageOptions.DontRequireReceiver);
                        CurrentItem = item;

                    }
                    else if (item.script.Type == InteractiveObject.Types.Consumable)
                    {
                        item.script.SendMessage("Consume", SendMessageOptions.DontRequireReceiver);

                    }
                }

            }

        }
        // check if the player has a camera
        public GameObject ReturnCameraItem()
        {
            foreach (ItemInInventoryOld item in Items)
            {

                var cameraScript = item.item.GetComponent<CameraSystem>();
                if (cameraScript != null)
                {
                    return item.item;
                }
            }
            return null;
        }
        // check if the player has a flashlight

        public GameObject ReturnFlashlightItem()
        {
            foreach (ItemInInventoryOld item in Items)
            {
                var cameraScript = item.item.GetComponent<Flashlight>();
                if (cameraScript != null)
                {
                    return item.item;
                }
            }
            return null;
        }

        public void StopUsing()
        {

            CurrentItem.script.SendMessage("StopUsing");
            CurrentItem = null;
            /*
            CurrentItem.item = null;
            CurrentItem.script = null;
            CurrentItem.canvasIcon = null;
            CurrentItem.weapon = null;
            */
        }
        void OnApplicationQuit()
        {
            Save();
        }
        public void Save()
        {
            foreach (var item in Items)
            {

                PlayerPrefs.SetString(item.script.ItemName, item.item.name);
                PlayerPrefs.SetInt(item.script.ItemName, 0);

            }
            if (CurrentItem != null && CurrentItem.script)
                PlayerPrefs.SetInt(CurrentItem.script.ItemName, 1);
            PlayerPrefs.Save();

        }

      /// player preferences
        public void DeleteData()
        {
            PlayerPrefs.DeleteAll();
            Reset();
        }
        public void DropItem(InteractiveObject itemP)
        {
            ItemInInventoryOld delete = null;
            foreach (var item in Items)
            {
                if (item.script == itemP)
                {
                    delete = item;
                    break;
                }
            }
            Items.RemoveAll(x => x.item == delete.item);

        }

        public void DropItem(ItemInInventoryOld itemP)
        {

            Items.RemoveAll(x => x.item == itemP.item);

        }
        // space to put the icon in inventory
        private Image GetFirstEmptyCanvasItem()
        {
            for (int i = 0; i < InventoryItemsIcons.Length; i++)
            {
                if (InventoryItemsIcons[i].sprite == null)
                {
                    return InventoryItemsIcons[i];
                }
            }
            return null;
        }

        public void Reset()
        {
            Items = new List<ItemInInventoryOld>();
            CurrentItem = null;

            ItemExaminingScript = null;
        }
    }
}
