using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor.Events;
namespace StrangeEngine
{
    [System.Serializable]
    public enum WeaponTypes
    {
        MachineGun = 0,
        Launcher = 1,
        GrenadeLauncher = 2,
        Laser = 3,
        Lightning = 4
    }
    public enum newOrPref
    {
        selectHowYouWantToAddWeapon,
        AddWeaponFromPrefab,
        MakeNewWeapon
    }
    public class MakeNewWeapon : EditorWindow
    {
        public WeaponTypes types;
        public newOrPref sele;
        int weaponIndex;
        int weaponAmmo;
        int weaponDamage;
        float WeaponAmmo;
        List<string> weapons;
        List<string> toggles;
        List<int> wepIndx;
        List<GameObject> WeaponGameObjects;
        Weapons w;
        GameObject g; // new weapon on weaponsSelection.
        GameObject p; // 'g' child.
        GameObject b; // new weapon on weapon Panel.
        GameObject pb; // 'b' child.
        GameObject t; // Toggle GameObject
        GameObject W;
        ActiveWeapons activeWeap;
        WeaponSelection weapSelect;
        StartWeapons startWeap;
        string weaponName;
        string toggleName;
        string WeaponToggleNumber;
        Vector2 scrollPos = Vector2.zero;
        Texture texture;
        Sprite WeaponImage;
        Sprite ToggleWeaponImage;
        private Object prefab;
        private GameObject pre;
        [MenuItem("Strange Engine/Weapon Maker", priority = 1)]
        public static void ShowWindow()
        {
            GetWindow<MakeNewWeapon>("Weapons");
        }
        void OnEnable()
        {
            string[] results1 = AssetDatabase.FindAssets("WeaponMaker");
            string textutrePath = AssetDatabase.GUIDToAssetPath(results1[0]);
            texture = (Texture)AssetDatabase.LoadAssetAtPath(textutrePath, typeof(Texture));

        }
        void OnGUI()
        {
            string[] results2 = AssetDatabase.FindAssets("_Weapons");
            string ScriptableObjectpath = AssetDatabase.GUIDToAssetPath(results2[0]);
            GUILayout.Label(texture, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(80));
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.MinHeight(10), GUILayout.MaxHeight(800));
            sele = (newOrPref)EditorGUILayout.EnumPopup("", sele);
            switch (sele)
            {
                case newOrPref.AddWeaponFromPrefab:
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    prefab = EditorGUILayout.ObjectField("Weapon Prefab:", prefab, typeof(Object), allowSceneObjects: true);
                    WeaponImage = (Sprite)EditorGUILayout.ObjectField("Add Weapon Sprite:", WeaponImage, typeof(Sprite), allowSceneObjects: true);
                    weaponDamage = EditorGUILayout.IntField("Weapon Damage:", weaponDamage);
                    weaponAmmo = EditorGUILayout.IntField("Weapon Start Ammo:", weaponAmmo);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("Add New Weapon", "adds new weapon.")))
                    {
                        createWeaponFromPrefab();
                        PrefabAddComponent();
                    }
                    if (GUILayout.Button(new GUIContent("Remove Last Weapon", "removes last added weapon.")))
                    {
                        if (EditorUtility.DisplayDialog("Remove Last Weapon", "Are shure you want to remove last added weapon?", "Yes", "No, it was a mistake"))
                        {
                            removeWeapon();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    if (weapons == null)
                    {

                        w = (Weapons)AssetDatabase.LoadAssetAtPath(ScriptableObjectpath, typeof(Weapons));
                        weapons = w.wep;
                    }
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        EditorGUILayout.SelectableLabel("weapon :  " + weapons[i]);
                    }
                    EditorGUILayout.HelpBox("You can always change damage value in damage array in GameManager.", MessageType.Info);
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    break;
                case newOrPref.MakeNewWeapon:

                    GUILayout.Label("New Weapon Settings", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
                    GUILayout.Label("Choose what type of weapon you want to add.", EditorStyles.centeredGreyMiniLabel);
                    types = (WeaponTypes)EditorGUILayout.EnumPopup("", types);
                    GUILayout.Label("Enter Weapon name you want to add.", EditorStyles.centeredGreyMiniLabel);
                    WeaponImage = (Sprite)EditorGUILayout.ObjectField("Add Weapon Sprite:", WeaponImage, typeof(Sprite), allowSceneObjects: true);
                    weaponName = EditorGUILayout.TextField("Weapon Name", "" + weaponName);
                    if (types == WeaponTypes.Launcher)
                    {
                        WeaponAmmo = EditorGUILayout.FloatField("Weapon Start Ammo:", WeaponAmmo);
                    }
                    else
                    {
                        weaponAmmo = EditorGUILayout.IntField("Weapon Start Ammo:", weaponAmmo);
                    }
                    weaponDamage = EditorGUILayout.IntField("Weapon Damage:", weaponDamage);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent("Add New Weapon", "adds new weapon.")))
                    {
                        createWeapon();
                        addComponent(types);
                    }
                    if (GUILayout.Button(new GUIContent("Remove Last Weapon", "removes last added weapon.")))
                    {
                        if (EditorUtility.DisplayDialog("Remove Last Weapon", "Are shure you want to remove last added weapon?", "Yes", "No, it was a mistake"))
                        {
                            removeWeapon();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    if (weapons == null)
                    {
                        w = (Weapons)AssetDatabase.LoadAssetAtPath(ScriptableObjectpath, typeof(Weapons));
                        weapons = w.wep;
                    }
                    for (int i = 0; i < weapons.Count; i++)
                    {
                        EditorGUILayout.SelectableLabel("weapon :  " + weapons[i]);
                    }
                    switch (types)
                    {
                        case WeaponTypes.MachineGun:
                            EditorGUILayout.HelpBox("After you add new weapon: " +
                         "\n1) Set All Settings in MachineGun component, attached to your new Weapon.", MessageType.Info);

                            break;
                        case WeaponTypes.Launcher:
                            EditorGUILayout.HelpBox("After you add new weapon: " +
                       "\n1) Set All Settings in Launcher component, attached to your new Weapon.", MessageType.Info);
                            break;
                        case WeaponTypes.GrenadeLauncher:
                            EditorGUILayout.HelpBox("After you add new weapon: " +
                        "\n1) Set All Settings in GrenadeLauncher component, attached to your new Weapon.", MessageType.Info);
                            break;
                        case WeaponTypes.Laser:
                            EditorGUILayout.HelpBox("After you add new weapon: " +
                        "\n1) Set All Settings in Laser component, attached to your new Weapon.", MessageType.Info);
                            break;
                        case WeaponTypes.Lightning:
                            EditorGUILayout.HelpBox("After you add new weapon: " +
                        "\n1) Set All Settings in Lightning component, attached to your new Weapon.", MessageType.Info);
                            break;
                        default:
                            Debug.LogError("Unrecognized Option");
                            break;
                    }
                    break;
                default:
                    break;
            }


            GUILayout.Label("New Toggles for Weapons", EditorStyles.boldLabel, GUILayout.ExpandWidth(true));
            EditorGUILayout.HelpBox("Before adding toggles, Open SelectionMenu Scene. ", MessageType.Info);
            ToggleWeaponImage = (Sprite)EditorGUILayout.ObjectField("Add Toggle Sprite:", ToggleWeaponImage, typeof(Sprite), allowSceneObjects: true);
            GUILayout.Label("Enter Toggle name.", EditorStyles.centeredGreyMiniLabel);
            toggleName = EditorGUILayout.TextField("Toggle Name", "" + toggleName);
            EditorGUILayout.HelpBox("'Toggle Weapon Number' is index number of weapon in weapon selection(count starts from 0, like in arrays)", MessageType.Info);
            WeaponToggleNumber = EditorGUILayout.TextField("Toggle Weapon Number", "" + WeaponToggleNumber);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent("Add New Toggle", "adds toggle and parents it to 'WeaponsPanel' ")))
            {

                if (EditorUtility.DisplayDialog("Add new Toggle", "Is current scene 'SelectionMenu'? (if not, nothing will work)", "Yep", "No"))
                {
                    createToggle();
                }
            }
            if (GUILayout.Button(new GUIContent("Remove Last Toggle", "removes last toggle from 'WeaponsPanel' ")))
            {
                if (EditorUtility.DisplayDialog("Remove last Toggle", "Is current scene 'SelectionMenu'? (if not, nothing will work)", "Yep", "No"))
                {
                    removeToggle();
                }
            }
            EditorGUILayout.EndHorizontal();
            if (toggles == null)
            {
                w = (Weapons)AssetDatabase.LoadAssetAtPath(ScriptableObjectpath, typeof(Weapons));
                toggles = w.toggles;
            }
            for (int i = 0; i < toggles.Count; i++)
            {
                toggles[i] = EditorGUILayout.TextField("Toggle", toggles[i]);
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.EndScrollView();
        }
        //
        void createWeaponFromPrefab()
        {

            pre = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            pre.name = prefab.name;
            weapons.Add(pre.name); // add new weapon to "Weapons" ScriptableObject.

            W = GameObject.FindWithTag("WeaponSelection");// finding weaponSelection to parent new weapon to it.
            pre.transform.SetParent(W.transform);
            //
            // instantiate weapon prefab and parent it to weapon.

            // 2
            //
            //adding new new Weapon on weapons panel.
            // creating new "Weapon" Gameobject, parent it to weapon panel and add Button and Image components.
            b = new GameObject("" + pre.name);
            b.AddComponent<Image>();
            if (WeaponImage)
            {
                b.GetComponent<Image>().sprite = WeaponImage;
                b.GetComponent<Image>().preserveAspect = true;
            }
            b.AddComponent<Button>();
            b.layer = 5;
            //Debug.Log("2");
            GameObject weaponpanel = GameObject.Find("Panel");// finding weapon Panel to parent new weapon to it.
            b.transform.SetParent(weaponpanel.transform);
            b.transform.localScale = new Vector3(1, 1, 1);
            //
            // create new Text GameObject and parent it to weapon.
            pb = new GameObject(pre.name + "Ammo");
            Debug.Log("3");
            pb.transform.SetParent(b.transform);
            pb.AddComponent<Text>();
            Debug.Log("4");
            Text text = pb.GetComponent<Text>();
            text.font = w.MainFont;
            text.fontSize = 30;
            text.color = new Color32(50, 50, 50, 255);
            text.alignment = TextAnchor.MiddleCenter;
            pb.layer = 5;
            RectTransform TEXTrect = pb.GetComponent<RectTransform>();
            TEXTrect.localScale = new Vector3(1, 1, 1);
            TEXTrect.anchorMin = new Vector2(0.6187f, 0f);
            TEXTrect.anchorMax = new Vector2(1, 1);
            TEXTrect.offsetMin = new Vector2(0, 0);
            TEXTrect.offsetMax = new Vector2(20, 0);
            //
            activeWeap = FindObjectOfType<ActiveWeapons>(); // Reference to ActiveWeapons.
            activeWeap.Weapons.Add(b); // Add Weapon to Activeweapons List.
            weaponIndex = activeWeap.Weapons.Count - 1;
            startWeap = FindObjectOfType<StartWeapons>(); // Reference to StartWeapons.
            startWeap.Weapons.Add(b); // Add Weapon to Activeweapons List.
                                      //
            Button butten = b.GetComponent<Button>();
            WeaponSelection waepsel = FindObjectOfType<WeaponSelection>();
            UnityAction<int> action = new UnityAction<int>(waepsel.Select);
            UnityEventTools.AddIntPersistentListener(butten.onClick, action, weaponIndex);
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.WeaponsDamage.Add(weaponDamage);
        }
        //
        void createWeapon()
        {
            //
            // 1
            //
            // creating new "Weapon" Gameobject, parent it to weapon selection and add weaponMobile component.
            g = new GameObject("" + weaponName);
            g.AddComponent<weaponHolder>();
            if (WeaponImage)
            {
                g.GetComponent<SpriteRenderer>().sprite = WeaponImage;
            }
            W = GameObject.FindWithTag("WeaponSelection");// finding weaponSelection to parent new weapon to it.
            g.transform.SetParent(W.transform);
            //
            // create new GameObject and parent it to weapon.
            p = new GameObject("GameObject");
            p.transform.SetParent(g.transform);
            //
            // add name to new weapon
            string www = weaponName;
            g.name = www;
            weapons.Add(g.name); // add new weapon to "Weapons" ScriptableObject.
                                 //
            // 2
            //
            //adding new new Weapon on weapons panel.
            // creating new "Weapon" Gameobject, parent it to weapon panel and add Button and Image components.
            b = new GameObject("" + weaponName);
            b.AddComponent<Image>();
            b.GetComponent<Image>().sprite = WeaponImage;
            b.AddComponent<Button>();
            b.layer = 5;
            GameObject weaponpanel = GameObject.Find("Panel");// finding weapon Panel to parent new weapon to it.
            b.transform.SetParent(weaponpanel.transform);
            b.transform.localScale = new Vector3(1, 1, 1);
            //
            // create new Text GameObject and parent it to weapon.
            string[] results2 = AssetDatabase.FindAssets("_Weapons");
            string ScriptableObjectpath = AssetDatabase.GUIDToAssetPath(results2[0]);
            w = (Weapons)AssetDatabase.LoadAssetAtPath(ScriptableObjectpath, typeof(Weapons));
            pb = new GameObject(weaponName + "Ammo");
            pb.transform.SetParent(b.transform);
            pb.AddComponent<Text>();
            Text text = pb.GetComponent<Text>();
            text.font = w.MainFont;
            text.fontSize = 30;
            text.color = new Color32(50, 50, 50, 255);
            text.alignment = TextAnchor.MiddleCenter;
            pb.layer = 5;
            RectTransform TEXTrect = pb.GetComponent<RectTransform>();
            TEXTrect.localScale = new Vector3(1, 1, 1);
            TEXTrect.anchorMin = new Vector2(0.6187f, 0f);
            TEXTrect.anchorMax = new Vector2(1, 1);
            TEXTrect.offsetMin = new Vector2(0, 0);
            TEXTrect.offsetMax = new Vector2(20, 0);
            //
            activeWeap = FindObjectOfType<ActiveWeapons>(); // Reference to ActiveWeapons.
            activeWeap.Weapons.Add(b); // Add Weapon to Activeweapons List.
            weaponIndex = activeWeap.Weapons.Count - 1;
            startWeap = FindObjectOfType<StartWeapons>(); // Reference to StartWeapons.
            startWeap.Weapons.Add(b); // Add Weapon to Activeweapons List.
                                      //
            Button butten = b.GetComponent<Button>();
            WeaponSelection waepsel = FindObjectOfType<WeaponSelection>();
            UnityAction<int> action = new UnityAction<int>(waepsel.Select);
            UnityEventTools.AddIntPersistentListener(butten.onClick, action, weaponIndex);
            GameManager gameManager = FindObjectOfType<GameManager>();
            gameManager.WeaponsDamage.Add(weaponDamage);
        }
        //
        void removeWeapon()
        {
            if (weapons.Count != 0)
            {
                string LastAdded = weapons[w.wep.Count - 1];
                //
                // 1
                //
                GameObject FindLastAdded = GameObject.Find("" + LastAdded);
                DestroyImmediate(FindLastAdded);
                //
                // 2
                //
                activeWeap = FindObjectOfType<ActiveWeapons>(); // Reference to ActiveWeapons.
                activeWeap.Weapons.RemoveAt(activeWeap.Weapons.Count - 1); // Remove weapon from ActiveWeapons List
                startWeap = FindObjectOfType<StartWeapons>(); // Reference to StartWeapons.
                startWeap.Weapons.RemoveAt(startWeap.Weapons.Count - 1); // Remove weapon from StartWeapons List
                GameObject FindLastAddedOnPanel = GameObject.Find("" + LastAdded);
                DestroyImmediate(FindLastAddedOnPanel);
                weapons.RemoveAt(weapons.Count - 1);
            }
        }
        //
        void PrefabAddComponent()
        {
            if (pre.GetComponentInChildren<MachineGun>())
            {
                pb.AddComponent<Ammo>();
                MachineGun[] machineG = pre.GetComponentsInChildren<MachineGun>();
                for (int i = 0; i < machineG.Length; i++)
                {
                    machineG[i].AmmoText = pb;
                }
                pb.GetComponent<Ammo>().startAmmo = weaponAmmo;
            }
            else if (pre.GetComponentInChildren<Launcher>())
            {
                pb.AddComponent<LauncherAmmo>();
                Launcher[] launch = pre.GetComponentsInChildren<Launcher>();
                for (int i = 0; i < launch.Length; i++)
                {
                    launch[i].AmmoText = pb;
                }
                pb.GetComponent<LauncherAmmo>().startAmmo = weaponAmmo;
            }
            else if (pre.GetComponentInChildren<GrenadeLauncher>())
            {
                pb.AddComponent<Ammo>();
                GrenadeLauncher[] gren = pre.GetComponentsInChildren<GrenadeLauncher>();
                for (int i = 0; i < gren.Length; i++)
                {
                    gren[i].AmmoText = pb;
                }
                pb.GetComponent<Ammo>().startAmmo = weaponAmmo;
            }
            else if (pre.GetComponentInChildren<LaserLauncher>())
            {
                pb.AddComponent<Ammo>();
                LaserLauncher[] las = pre.GetComponentsInChildren<LaserLauncher>();
                for (int i = 0; i < las.Length; i++)
                {
                    las[i].AmmoText = pb;
                }
                pb.GetComponent<Ammo>().startAmmo = weaponAmmo;
            }
            else if (pre.GetComponentInChildren<LightningBolt>())
            {
                pb.AddComponent<Ammo>();
                LightningBolt[] ligh = pre.GetComponentsInChildren<LightningBolt>();
                for (int i = 0; i < ligh.Length; i++)
                {
                    ligh[i].AmmoText = pb;
                }
                pb.GetComponent<Ammo>().startAmmo = weaponAmmo;
            }
            else
            {
                Debug.LogError("Unrecognized Option");
            }
        }
        /// <summary>
        /// Adds the component of wepon type.
        /// </summary>
        /// <param name="types">Types.</param>
        void addComponent(WeaponTypes types)
        {
            switch (types)
            {
                case WeaponTypes.MachineGun:
                    p.AddComponent<MachineGun>();
                    pb.AddComponent<Ammo>();
                    p.GetComponent<MachineGun>().AmmoText = pb;
                    pb.GetComponent<Ammo>().startAmmo = weaponAmmo;

                    break;
                case WeaponTypes.Launcher:
                    p.AddComponent<Launcher>();
                    pb.AddComponent<LauncherAmmo>();
                    p.GetComponent<Launcher>().AmmoText = pb;
                    pb.GetComponent<LauncherAmmo>().startAmmo = WeaponAmmo;
                    break;
                case WeaponTypes.GrenadeLauncher:
                    p.AddComponent<GrenadeLauncher>();
                    pb.AddComponent<Ammo>();
                    p.GetComponent<GrenadeLauncher>().AmmoText = pb;
                    pb.GetComponent<Ammo>().startAmmo = weaponAmmo;

                    break;
                case WeaponTypes.Laser:
                    p.AddComponent<LaserLauncher>();
                    pb.AddComponent<Ammo>();
                    p.GetComponent<LaserLauncher>().AmmoText = pb;
                    pb.GetComponent<Ammo>().startAmmo = weaponAmmo;
                    break;
                case WeaponTypes.Lightning:
                    p.AddComponent<LightningBolt>();
                    pb.AddComponent<Ammo>();
                    p.GetComponent<LightningBolt>().AmmoText = pb;
                    pb.GetComponent<Ammo>().startAmmo = weaponAmmo;
                    break;
                default:
                    Debug.LogError("Unrecognized Option");
                    break;
            }
        }
        //
        void createToggle()
        {
            t = PrefabUtility.InstantiatePrefab(w.prefabs[0]) as GameObject;
            // add name to new weapon
            string ttt = toggleName;
            t.name = ttt;
            toggles.Add(t.name); // add new toggle to "Weapons" ScriptableObject.
                                 //t = new
            GameObject WeaponsPanel = GameObject.Find("WeaponsPanel");
            t.transform.SetParent(WeaponsPanel.transform);
            WeaponsPanel.GetComponent<MyToggleGroup>().toggles.Add(t.GetComponent<Toggle>());
            int chc = t.transform.childCount;
            Transform Chc = t.transform.GetChild(chc - 1);
            Chc.GetComponent<Image>().sprite = ToggleWeaponImage;
            Toggle tuggen = t.GetComponent<Toggle>();
            SelectionMenu SelMenu = FindObjectOfType<SelectionMenu>();
            UnityAction<string> action = new UnityAction<string>(SelMenu.SelectWeapon);
            UnityEventTools.AddStringPersistentListener(tuggen.onValueChanged, action, WeaponToggleNumber);
            PrefabUtility.DisconnectPrefabInstance(t);
        }
        //
        void removeToggle()
        {
            if (w.toggles.Count != 0)
            {
                string LastAddedToggle = toggles[w.toggles.Count - 1];
                MyToggleGroup m = GameObject.Find("WeaponsPanel").GetComponent<MyToggleGroup>();
                m.toggles.RemoveAt(m.toggles.Count - 1);
                //
                // 1
                //
                GameObject FindLastAddedToggle = GameObject.Find("" + LastAddedToggle);
                DestroyImmediate(FindLastAddedToggle);
                toggles.RemoveAt(toggles.Count - 1);
            }
        }
    }
}