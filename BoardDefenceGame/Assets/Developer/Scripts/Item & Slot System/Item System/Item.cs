using DG.Tweening;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public GameObject rangeSpriteGO;
    public float totalHealth;
    public float itemMaxHealth = 100;
    public float itemCurrentHealth;
    public ItemBrokenList itemBrokenList;
    private static GameObject selectedObject = null;
    public GameObject glowGO;
    public int selectionProbability = 1;
    public int coinValue = 7;
    public int slotCount = 1;
    public int itemId = 0;
    public int itemMergeLevel = 0;
    public int itemMaxLevel = 2;

    public string itemName;
    public string itemExplainName;
    public string itemTypeName;
    public int spriteAngleValue = 0;

    public List<GameObject> break_1_List = new List<GameObject>();
    public List<GameObject> break_2_List = new List<GameObject>();
    public List<GameObject> break_3_List = new List<GameObject>();
    public List<Image> itemCooldownImages;
    public List<GameObject> itemScaleAnimList;

    public List<GameObject> itemSpriteList;
    public List<Sprite> itemImgSpriteList;
    public List<GunSystem> itemGunList;
    public List<Item> mergableItems;
    public Item targetItem;

    public List<Slot> targetSlotList = new List<Slot>();
    public Slot targetSlot;
    public ItemCreatorSubPanel itemCreatorSubPanel;

    public bool isDragging = false;
    public bool isDragged = false;
    private Vector3 offset;
    private Camera mainCamera;
    private int draggableLayerMask;

    public bool collisionActive = true;
    public Animator itemAnimation;

    public List<GameObject> mergeParticleList;

    public void TotalHealthInitialize()
    {
        totalHealth = itemMaxHealth;
    }
    public void HealthRegen(float regenAmount)
    {
        float diff = itemCurrentHealth + ((totalHealth / 100f) * regenAmount);

        if (itemCurrentHealth < totalHealth)
        {
            if (diff < totalHealth)
            {
                itemCurrentHealth = diff;
            }
            else
            {
                itemCurrentHealth = totalHealth;
            }
        }
        float healthDifRate = itemCurrentHealth / totalHealth;
        HealthRateCheck(healthDifRate);
    }
    public void ItemInitialize()
    {

        foreach (var itemSprt in itemSpriteList)
        {
            itemSprt.SetActive(false);
        }
        itemSpriteList[itemMergeLevel].SetActive(true);
    }
    public void MergeThis()
    {
        OrderNoCanvas(510 - (int)transform.position.y);

        itemCurrentHealth = totalHealth;
        foreach (var brk in break_1_List)
        {
            brk.SetActive(false);
        }
        foreach (var brk in break_2_List)
        {
            brk.SetActive(false);
        }
        foreach (var brk in break_3_List)
        {
            brk.SetActive(false);
        }

        if (mergeParticleList.Count > 0)
        {
            mergeParticleList[itemMergeLevel - 1].SetActive(true);
        }

        if(targetSlot != null)
        {
            targetSlot.slotPanel.UpdateInsideGuns();
        }
        SpriteCooldown(0f, 1f);
        if (itemScaleAnimList[itemMergeLevel].GetComponent<ScaleAnimation>() != null)
        {
            itemScaleAnimList[itemMergeLevel].GetComponent<ScaleAnimation>().enabled = false;
        }
        SlotManager.Instance.AllSlotFullCheck();
    }
    void Start()
    {
        itemBrokenList.gameObject.SetActive(false);

        TotalHealthInitialize();

        itemCurrentHealth = totalHealth;

        firstScale = transform.localScale;
        ItemInitialize();
        mainCamera = Camera.main;
        draggableLayerMask = LayerMask.GetMask("Draggable");
        SpriteCooldown(0f, 1f);
    }
    void HealthRateCheck(float healthDifRate)
    {
        if (healthDifRate >= 0.8f)
        {
            foreach (var brk in break_1_List)
            {
                brk.SetActive(false);
            }
            foreach (var brk in break_2_List)
            {
                brk.SetActive(false);
            }
            foreach (var brk in break_3_List)
            {
                brk.SetActive(false);
            }
        }
        else if (healthDifRate >= 0.5f)
        {
            foreach (var brk in break_1_List)
            {
                brk.SetActive(true);
            }
        }
        else if (healthDifRate >= 0.2f)
        {
            foreach (var brk in break_2_List)
            {
                brk.SetActive(true);
            }
        }
        else
        {
            foreach (var brk in break_3_List)
            {
                brk.SetActive(true);
            }
        }
    }
    public void GetDamage(int damage)
    {
        itemCurrentHealth -= damage;
        itemCurrentHealth = Mathf.Clamp(itemCurrentHealth, 0, totalHealth);
        float healthDifRate = itemCurrentHealth / totalHealth;

        HealthRateCheck(healthDifRate);

        if (itemCurrentHealth <= 0)
        {
            ItemDeath();
        }
    }
    void ItemDeath()
    {
        SlotOutItem();
        itemBrokenList.gameObject.SetActive(true);
        itemBrokenList.ItemBrake();
        Destroy(gameObject);
    }
    IEnumerator ColliderReActivator()
    {
        GetComponent<Collider2D>().enabled = false;
        yield return null;
        GetComponent<Collider2D>().enabled = true;
    }
    Vector3 mousePos;
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Globals.itemClickActive)
        {
            mousePos = GetMouseWorldPosition();

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero, Mathf.Infinity, draggableLayerMask);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                if (selectedObject == null)
                {
                    isDragged = false;

                    selectedObject = gameObject;
                    isDragging = true;
                    offset = transform.position - mousePos;

                    OrderNoCanvas(520);
                    ElasticScale(1f, 1.4f, 0.5f, Ease.Flash);
                    if (itemScaleAnimList[itemMergeLevel].GetComponent<ScaleAnimation>() != null)
                    {
                        itemScaleAnimList[itemMergeLevel].GetComponent<ScaleAnimation>().enabled = true;
                    }
                    if(glowGO != null)
                    {
                        glowGO.SetActive(true);
                    }
                }
            }
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            transform.position = Vector3.Lerp(transform.position, GetMouseWorldPosition() + offset, Time.deltaTime * 20);
            if (!isDragged && mousePos != GetMouseWorldPosition())
            {
                isDragged = true;
                if (selectedObject != null)
                {
                    SlotOutItem();
                    rangeSpriteGO.SetActive(true);
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            OrderNoCanvas(510);
            ElasticScale(1.4f, 1f, 1f, Ease.OutElastic);
            rangeSpriteGO.SetActive(false);

            if (itemScaleAnimList[itemMergeLevel].GetComponent<ScaleAnimation>() != null)
            {
                itemScaleAnimList[itemMergeLevel].GetComponent<ScaleAnimation>().enabled = false;
            }
            if (glowGO != null)
            {
                glowGO.SetActive(false);
            }
            selectedObject = null;

            isDragging = false;
            if (mousePos != GetMouseWorldPosition())
            {
                foreach (var _slt in targetSlotList)
                {
                    _slt.TransparentColorInit();
                }
                if (mergableItems.Count > 0)
                {
                    MergeItems(itemMergeLevel);
                    foreach (var _slt in targetSlotList)
                    {
                        _slt.TransparentColorInit();
                    }
                }
                else if (targetSlotList.Count >= slotCount && TargetSlotsCheck())
                {
                    DropInSlot_Item();
                }
                else
                {
                    GoSlot();
                }
            }
        }
    }
    bool TargetSlotsCheck()
    {
        bool dropActive = true;
        int sltId = targetSlotList[0].slotID;

        for(int i = 0; i < targetSlotList.Count; i++)
        {
            if(sltId != targetSlotList[i].slotID)
            {
                dropActive = false;
                break;
            }
            sltId = targetSlotList[i].slotID;
        }
        return dropActive;
    }
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Slot>() != null)
        {
            Slot slot = collision.GetComponent<Slot>();
            if (slot != null && !targetSlotList.Contains(slot))
            {
                targetSlotList.Add(slot);

                if (targetSlotList.Count >= slotCount && TargetSlotsCheck())
                {
                    foreach(var _slt in targetSlotList)
                    {
                        _slt.SelectableColorInit();
                    }
                }
            }
        }
        if (collision.GetComponent<Item>() != null && isDragging)
        {
            Item collisionItem = collision.GetComponent<Item>();
            if(CheckMergable(this , collisionItem) && collisionItem.collisionActive)
            {
                mergableItems.Add(collisionItem);
            }
        }

    }
    bool CheckMergable(Item thisItem , Item otherItem)
    {
        bool mergableActive = false;
        if (otherItem.itemId == thisItem.itemId && (thisItem.itemMergeLevel + otherItem.itemMergeLevel + 1) < itemMaxLevel)
        {
            mergableActive = true;
        }
        return mergableActive;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Slot>() != null)
        {
            collision.GetComponent<Slot>().NormalColorInit();
            targetSlotList.Remove(collision.GetComponent<Slot>());
        }
        if (collision.GetComponent<Item>() != null && isDragging)
        {
            Item collisionItem = collision.GetComponent<Item>();

            if (collisionItem.itemId == itemId && collisionItem.itemMergeLevel == itemMergeLevel && itemMergeLevel < itemMaxLevel)
            {
                mergableItems.Remove(collisionItem);
            }
        }
    }
    public void SlotOutItem()
    {
        transform.parent = SlotManager.Instance.transform.parent;
        collisionActive = true;
        if (targetSlot != null && targetSlot.currentItem != null)
        {
            if (targetSlot.currentItem.gameObject == gameObject)
            {
                targetSlot.slotPanel.itemInsideSlot.Remove(this);
                targetSlot.slotPanel.RemoveInsideGunsSlot(this);
                targetSlot.slotPanel.UpdateInsideGuns();
                SlotManager.Instance.allItemSlot.Remove(this);

                foreach (var slt in targetSlotList)
                {
                    slt.currentItem = null;
                }
            }
        }
        StartCoroutine(ColliderReActivator());
        SpriteCooldown(0f, 1f);
        SlotManager.Instance.AllSlotFullCheck();
    }
   public void DropInSlot_Item()
    {
        StartCoroutine(DropMove());
    }
    IEnumerator DropMove()
    {
        collisionActive = false;
        Slot tempSlot = null;
        if (targetSlot != null)
        {
            tempSlot = targetSlot;
        }
        targetSlot = CheckNearestSlot();

        bool anySlotsIsFull = false;
        foreach (var slt in targetSlotList)
        {
            if (slt.currentItem != null)
            {
                anySlotsIsFull = true;
                if (CheckMergable(this, slt.currentItem))
                {
                    mergableItems.Add(slt.currentItem);
                    MergeItems(itemMergeLevel);
                    yield break;
                }
            }
        }

        if (anySlotsIsFull)
        {
            foreach (var slt in targetSlotList)
            {
                if (slt.currentItem != null)
                {
                    slt.currentItem.GoSlot();
                    slt.slotPanel.itemInsideSlot.Remove(slt.currentItem);
                    slt.slotPanel.RemoveInsideGunsSlot(slt.currentItem);
                    slt.slotPanel.UpdateInsideGuns();
                    SlotManager.Instance.allItemSlot.Remove(slt.currentItem);

                    foreach (var _slt in slt.currentItem.targetSlotList)
                    {
                        if (_slt.currentItem != null)
                        {
                            _slt.currentItem = null;
                        }
                    }
                }
            }
        }

        float _speed = 8f;
        float counter = 0f;
        Vector2 firstPos = transform.position;
        while (counter < 1f)
        {
            counter += _speed * Time.deltaTime;
            Mathf.Clamp01(counter);
            transform.position = Vector2.Lerp(firstPos, targetSlot.transform.position, counter);
            yield return null;
        }
        OrderNoCanvas(510 - (int)transform.position.y);
        foreach (var slt in targetSlotList)
        {
            slt.currentItem = this;
        }

        targetSlot.slotPanel.itemInsideSlot.Add(this);
        targetSlot.slotPanel.AddInsideSlotGuns(this);
        targetSlot.slotPanel.UpdateInsideGuns();

        SlotManager.Instance.allItemSlot.Add(this);

        SlotManager.Instance.AllSlotFullCheck();
    }
    private Slot CheckNearestSlot()
    {
        Slot _targetSlot = targetSlotList[0];
        float distance = Vector3.Distance(_targetSlot.transform.position, transform.position);
        for (int i = 0; i < targetSlotList.Count; i++)
        {
            if (Vector3.Distance(targetSlotList[i].transform.position, transform.position) < distance)
            {
                distance = Vector3.Distance(targetSlotList[i].transform.position, transform.position);
                _targetSlot = targetSlotList[i];
            }
        }
        return _targetSlot;
    }
    void MergeItems(int thisLevel)
    {
        StartCoroutine(GoMerge(thisLevel));
    }
    IEnumerator GoMerge(int thisLevel)
    {
        targetItem = CheckNearestItem();
        float _speed = 8f;
        float counter = 0f;
        Vector2 firstPos = transform.position;
        while (counter < 1f)
        {
            counter += _speed * Time.deltaTime;
            Mathf.Clamp01(counter);
            transform.position = Vector2.Lerp(firstPos, targetItem.transform.position, counter);
            yield return null;
        }
        targetItem.itemMergeLevel += (thisLevel + 1);
        targetItem.ItemInitialize();
        targetItem.MergeThis();
        Destroy(gameObject);
    }
    private Item CheckNearestItem()
    {
        Item _targetItem = mergableItems[0];
        float distance = Vector3.Distance(_targetItem.transform.position, transform.position);
        for (int i = 0; i < mergableItems.Count; i++)
        {
            if (Vector3.Distance(mergableItems[i].transform.position, transform.position) < distance)
            {
                distance = Vector3.Distance(mergableItems[i].transform.position, transform.position);
                _targetItem = mergableItems[i];
            }
        }
        return _targetItem;
    }

    public void GoSubPanel()
    {
        StartCoroutine(GoSub_Panel());
    }
    IEnumerator GoSub_Panel()
    {
        Transform targetPosTR = CheckNearestTR();
        Vector2 impulseDirection = (targetPosTR.position - transform.position).normalized;

        float _speed = 2f;
        float counter = 0f;
        float counter2 = 0f;
        float posY = 0f;
        float posY_Factor = 2f;
        float angle = 0f;

        Vector2 firstPos = transform.position;
        Vector3 prePos = transform.position;
        while (counter < 1f)
        {
            counter += _speed * Time.deltaTime;
            counter2 += _speed * Time.deltaTime;
            Mathf.Clamp01(counter);
            angle = counter * Mathf.PI;
            posY = posY_Factor * Mathf.Sin(angle);

            Vector3 trgtPos = Vector3.Lerp(firstPos, targetPosTR.position, counter);
            trgtPos.y += posY;

            transform.position = trgtPos;

            impulseDirection = (transform.position - prePos).normalized;
            if (counter2 > 0.1f)
            {
                counter2 = 0f;
                prePos = transform.position;
            }
            yield return null;
        }
        ItemImpulse(impulseDirection, 800f);
    }
    Transform CheckNearestTR()
    {
        float distance = Vector3.Distance(ItemCreatorSubPanel.Instance.itemSubSlotPosListTR[0].transform.position, transform.position);
        Transform targetTR = ItemCreatorSubPanel.Instance.itemSubSlotPosListTR[0].transform;
        for (int i = 0; i < ItemCreatorSubPanel.Instance.itemSubSlotPosListTR.Count; i++)
        {
            if (Vector3.Distance(ItemCreatorSubPanel.Instance.itemSubSlotPosListTR[i].transform.position, transform.position) < distance)
            {
                distance = Vector3.Distance(ItemCreatorSubPanel.Instance.itemSubSlotPosListTR[i].transform.position, transform.position);
                targetTR = ItemCreatorSubPanel.Instance.itemSubSlotPosListTR[i].transform;
            }
        }
        return targetTR;
    }
    public void ConvertToMoney()
    {
        MoneyManager.Instance.MoneyCreate(transform.position, coinValue * (itemMergeLevel + 1), 1, true , true);
    }
    public void ItemImpulse(Vector2 _impulseDirection , float impulseForce)
    {
        GetComponent<Rigidbody2D>().AddForce(_impulseDirection * impulseForce);
    }

    public void PhysicsDeactived_Delay()
    {
        StartCoroutine(PhysicsDeactive_Delay());
    }
    IEnumerator PhysicsDeactive_Delay()
    {
        yield return new WaitForSeconds(2f);
        //GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
    }
    Vector3 firstScale;

    public Tween ElasticScale(float value, float lastValue, float duration, DG.Tweening.Ease type)
    {

        Tween tween = DOTween.To
            (() => value, x => value = x, lastValue, duration).SetEase(type).OnUpdate(delegate ()
            {
                foreach(var itm in itemSpriteList)
                {
                    itm.transform.localScale = Vector3.one * value;
                }
                //transform.localScale = firstScale * value;
            }).OnComplete(delegate ()
            {

            });
        return tween;
    }

    public void SpriteCooldown(float currentCooldown, float maxCooldown)
    {
        foreach(var img in itemCooldownImages)
        {
            img.fillAmount = currentCooldown / maxCooldown;
        }
    }
    public void ItemIconTurnTarget(Vector3 targetPos)
    {
        Vector3 direction = (targetPos) - transform.position;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg * 1f;
    }

    public void GoSlot()
    {
        StartCoroutine(GloSlotDelay(SelectSlot()));
    }
    IEnumerator GloSlotDelay(Slot _slot)
    {
        if (_slot != null)
        {
            Vector3 firstPos = transform.position;
            Vector3 targetPos = _slot.transform.position;
            float counter = 0f;
            float speed = 4f;

            while (counter < 1f)
            {
                counter += speed * Time.deltaTime;
                counter = Mathf.Clamp01(counter);

                transform.position = Vector3.Lerp(firstPos, targetPos, counter);
                yield return null;
            }
            DropInSlot_Item();
            ElasticScale(1.4f, 1f, 1f, Ease.OutElastic);

            if (itemScaleAnimList[itemMergeLevel].GetComponent<ScaleAnimation>() != null)
            {
                itemScaleAnimList[itemMergeLevel].GetComponent<ScaleAnimation>().enabled = false;
            }
            yield return null;
        }
    }

    Slot SelectSlot()
    {
        List<Slot> slotList = new List<Slot>();
        Slot selectedSlot = null;
        foreach (var sltPanel in SlotManager.Instance.slotPanels)
        {
            if (sltPanel.allSlots[0].currentItem == null)
            {
                slotList.Add(sltPanel.allSlots[0]);
            }
        }
        if (slotList.Count > 0)
        {
            float distance = Vector3.Distance(slotList[0].transform.position, ItemCreatorSubPanel.Instance.itemCreatePos.position);

            for (int i = 0; i < slotList.Count; i++)
            {
                if (distance >= Vector3.Distance(slotList[i].transform.position, ItemCreatorSubPanel.Instance.itemCreatePos.position) && slotList[i].currentItem == null)
                {
                    distance = Vector3.Distance(slotList[i].transform.position, ItemCreatorSubPanel.Instance.itemCreatePos.position);
                    selectedSlot = slotList[i];
                }
            }
        }
        return selectedSlot;
    }
    public void OrderNoCanvas(int orderNo)
    {
        foreach(var cnv in itemScaleAnimList)
        {
            cnv.GetComponent<Canvas>().sortingOrder = orderNo;
        }
    }
}
