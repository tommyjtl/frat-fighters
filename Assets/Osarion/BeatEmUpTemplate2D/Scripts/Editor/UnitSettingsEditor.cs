using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

namespace BeatEmUpTemplate2D {

        //custom editor for the Unit Settings component
        [CanEditMultipleObjects]
        [CustomEditor(typeof(UnitSettings))]
        class UnitSettingsEditor : Editor {

        //the dictionary is static to prevent strange behavior when editing settings directly from the Project folder (which causes editor updates to reset these bools)
        public static Dictionary<string, bool> foldOutList = new Dictionary<string, bool> {
            { "linkedComponentsFoldout", false },
            { "movementFoldout", false },
            { "jumpFoldout", false },
            { "attackDataFoldout", false },
            { "comboDataFoldout", false },
            { "knockDownFoldout", false },
            { "throwFoldout", false },
            { "defenceFoldout", false },
            { "grabFoldout", false },
            { "weaponFoldout", false },
            { "unitNameFoldout", false },
            { "fovFoldout", false },
        };    

        //cache serialized properties fields (for multi-editing support)
        private SerializedProperty[] properties;
        private HashSet<string> linkedComponentFields = new HashSet<string>{ "shadowPrefab", "weaponBone", "hitEffect", "hitBox", "spriteRenderer" };
        private HashSet<string> movementFields = new HashSet<string>{ "startDirection", "moveSpeed", "moveSpeedAir", "useAcceleration" };
        private HashSet<string> accelerationFields = new HashSet<string>{ "moveAcceleration", "moveDeceleration" };
        private HashSet<string> jumpFields = new HashSet<string>{ "jumpHeight", "jumpSpeed", "jumpGravity" };
        private HashSet<string> attackDataFields = new HashSet<string> { "jumpPunch", "jumpKick", "grabPunch", "grabKick", "grabThrow", "groundPunch", "groundKick" };
        private HashSet<string> comboDataFields = new HashSet<string> { "comboResetTime", "continueComboOnHit" };
        private HashSet<string> knockdownFields = new HashSet<string> { "knockDownHeight", "knockDownDistance", "knockDownSpeed", "knockDownFloorTime", "hitOtherEnemiesDuringFall", "hitOtherEnemiesWhenFalling" };
        private HashSet<string> throwFields = new HashSet<string> { "throwHeight", "throwDistance", "hitOtherEnemiesWhenThrown" };
        private HashSet<string> defenceFieldsPlayer = new HashSet<string> { "canChangeDirWhileDefending", "rearDefenseEnabled" };
        private HashSet<string> defenceFieldsEnemy = new HashSet<string> { "defendChance", "defendDuration", "rearDefenseEnabled" };
        private HashSet<string> grabFields = new HashSet<string> { "grabAnimation", "grabPosition", "grabDuration" };
        private HashSet<string> weaponFields = new HashSet<string> { "loseWeaponWhenHit", "loseWeaponWhenKnockedDown" };
        private HashSet<string> unitNameFieldsPlayer = new HashSet<string> { "unitName", "unitPortrait", "showNameInAllCaps" };
        private HashSet<string> unitNameFieldsEnemy = new HashSet<string> { "unitName", "showNameInAllCaps", "unitPortrait", "loadRandomNameFromList" };
        private HashSet<string> fovFields = new HashSet<string> { "enableFOV", "viewDistance", "viewAngle", "viewPosOffset", "showFOVCone", "targetInSight" };
        
        //icons
        private Texture2D iconArrowClose;
        private Texture2D iconArrowOpen;
        private Texture2D iconInfo;

        //other
        private DIRECTION prevDirection = DIRECTION.LEFT; //used to keep track of direction changes in the editor
        private string newline = "\n\n";
        private string space = "  ";

        void OnEnable() {

            //load icons
            iconArrowClose = Resources.Load<Texture2D>("IconArrowClose");
            iconArrowOpen = Resources.Load<Texture2D>("IconArrowOpen");
            iconInfo = Resources.Load<Texture2D>("IconInfo");

            //Get all serialized properties and cache them
            CacheSerializedProperties();
        }

         public override void OnInspectorGUI() {
            var settings = (UnitSettings)target;
            if (settings == null) return;

            //activate undo
            Undo.RecordObject(settings, "Undo change settings");

            //begin checking for changes
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            //draw main content
            MainContent(settings);
            
            //save changes
            if(EditorGUI.EndChangeCheck()){
                 serializedObject.ApplyModifiedProperties();
                 EditorUtility.SetDirty(settings);
            }
        }

        void MainContent(UnitSettings settings){

            //unit type
            DrawPropertyField("unitType");

            //linked components
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button(new GUIContent(space + "Linked Components", GetArrow(foldOutList["linkedComponentsFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["linkedComponentsFoldout"]  = !foldOutList["linkedComponentsFoldout"];
            if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(0, settings.unitType);
            EditorGUILayout.EndHorizontal();

            if(foldOutList["linkedComponentsFoldout"]) {
                EditorGUI.indentLevel++;
                DrawPropertyFields(linkedComponentFields);
                EditorGUI.indentLevel--;
            }

            //movement Settings
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button(new GUIContent(space + "Movement Settings", GetArrow(foldOutList["movementFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["movementFoldout"] = !foldOutList["movementFoldout"];
            if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(1, settings.unitType);
            EditorGUILayout.EndHorizontal();
            if(foldOutList["movementFoldout"]) {
                EditorGUI.indentLevel++;
                DrawPropertyFields(movementFields);

                //rotate unit in the level
                if(prevDirection != settings.startDirection){
                    settings.transform.localRotation = (settings.startDirection == DIRECTION.LEFT)? Quaternion.Euler(0,180,0) : Quaternion.identity;
                    prevDirection = settings.startDirection;
                }

                if(settings.useAcceleration){
                    EditorGUI.indentLevel++;
                    DrawPropertyFields(accelerationFields);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }

            //jump Settings
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button(new GUIContent(space + "Jump Settings", GetArrow(foldOutList["jumpFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["jumpFoldout"] = !foldOutList["jumpFoldout"];
            if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(2, settings.unitType);
            EditorGUILayout.EndHorizontal();
            if(foldOutList["jumpFoldout"]) {
                EditorGUI.indentLevel++;
                DrawPropertyFields(jumpFields);
                EditorGUI.indentLevel--;
            }

            //attack Data (Player)
            if(settings.unitType == UNITTYPE.PLAYER){
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button(new GUIContent(space + "Attack Data", GetArrow(foldOutList["attackDataFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["attackDataFoldout"] = !foldOutList["attackDataFoldout"];
                if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(3, settings.unitType);
                EditorGUILayout.EndHorizontal();

                //show attack data
                if(foldOutList["attackDataFoldout"]){
                    EditorGUI.indentLevel++;
                    foreach(string attack in attackDataFields) ShowAttackData(GetPropertyByName(attack), false);
                    EditorGUI.indentLevel--;
                }

                //combo Data
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button(new GUIContent(space + "Combo Data", GetArrow(foldOutList["comboDataFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["comboDataFoldout"] = !foldOutList["comboDataFoldout"];
                if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(4, settings.unitType);
                EditorGUILayout.EndHorizontal();

                if(foldOutList["comboDataFoldout"]){
                    EditorGUI.indentLevel++;
                    EditorGUILayout.Space(5);
                    ShowHeader("Combo Settings");
                    DrawPropertyFields(comboDataFields);
                    EditorGUILayout.Space(5);
                    ShowHeader("Combo List");
                    ShowComboData(settings.comboData);
                    EditorGUI.indentLevel--;
                }
            }

            //attack Data (Enemy)
            if(settings.unitType == UNITTYPE.ENEMY){
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button(new GUIContent(space + "Attack Data", GetArrow(foldOutList["attackDataFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["attackDataFoldout"] = !foldOutList["attackDataFoldout"];
                if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(3, settings.unitType);
                EditorGUILayout.EndHorizontal();

                if(foldOutList["attackDataFoldout"]){
                    EditorGUI.indentLevel++;
                    if(settings.unitType == UNITTYPE.ENEMY) DrawPropertyField("enemyPauseBeforeAttack");
                    EditorGUILayout.Space(5);
                    ShowHeader("Enemy Attack List");
                    ShowEnemyAttackData();                  
                    EditorGUI.indentLevel--;
                }
            }

            //knockDown Settings
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button(new GUIContent(space + "KnockDown Settings", GetArrow( foldOutList["knockDownFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30)))  foldOutList["knockDownFoldout"] = !foldOutList["knockDownFoldout"];
            if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(5, settings.unitType);
            EditorGUILayout.EndHorizontal();

            if(foldOutList["knockDownFoldout"]) {
                EditorGUI.indentLevel++;
                DrawPropertyField("canBeKnockedDown");
                if(settings.canBeKnockedDown) DrawPropertyFields(knockdownFields);
                EditorGUI.indentLevel--;
            }

            //throw Settings
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button(new GUIContent(space + "Throw Settings", GetArrow(foldOutList["throwFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["throwFoldout"] = !foldOutList["throwFoldout"];
            if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(6, settings.unitType);
            EditorGUILayout.EndHorizontal();     
            
            if(foldOutList["throwFoldout"]) {
                EditorGUI.indentLevel++;
                DrawPropertyFields(throwFields);
                EditorGUI.indentLevel--;
            }

            //defence Settings
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button(new GUIContent(space + "Defence Settings", GetArrow(foldOutList["defenceFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["defenceFoldout"] = !foldOutList["defenceFoldout"];
            if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(7, settings.unitType);
            EditorGUILayout.EndHorizontal();

            if(foldOutList["defenceFoldout"]) {
                EditorGUI.indentLevel++;
                if(settings.unitType == UNITTYPE.ENEMY) DrawPropertyFields(defenceFieldsEnemy);
                else if(settings.unitType == UNITTYPE.PLAYER) DrawPropertyFields(defenceFieldsPlayer);
                EditorGUI.indentLevel--;
            }

            //grab Settings 
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button(new GUIContent(space + "Grab Settings", GetArrow(foldOutList["grabFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["grabFoldout"] = !foldOutList["grabFoldout"];
            if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(8, settings.unitType);
            EditorGUILayout.EndHorizontal();

            if(foldOutList["grabFoldout"]) {
                EditorGUI.indentLevel++;
                if(settings.unitType == UNITTYPE.PLAYER) DrawPropertyFields(grabFields);
                if(settings.unitType == UNITTYPE.ENEMY) DrawPropertyField("canBeGrabbed");
                EditorGUI.indentLevel--;
            }

            //weapon Settings
            if(settings.unitType == UNITTYPE.PLAYER){
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button(new GUIContent(space + "Weapon Settings", GetArrow( foldOutList["weaponFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["weaponFoldout"] = !foldOutList["weaponFoldout"];
                if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(9, settings.unitType);
                EditorGUILayout.EndHorizontal();

                if(foldOutList["weaponFoldout"]){
                    EditorGUI.indentLevel++;
                    DrawPropertyFields(weaponFields);
                    EditorGUI.indentLevel--;
                }
            }

            //unit Name
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button(new GUIContent(space + "Unit Name & Portrait", GetArrow(foldOutList["unitNameFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["unitNameFoldout"] = !foldOutList["unitNameFoldout"];
            if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(10, settings.unitType);
            EditorGUILayout.EndHorizontal();

            if(foldOutList["unitNameFoldout"]) {
                EditorGUI.indentLevel++;
                if(settings.unitType == UNITTYPE.PLAYER) DrawPropertyFields(unitNameFieldsPlayer);             
                if(settings.unitType == UNITTYPE.ENEMY){
                    DrawPropertyFields(unitNameFieldsEnemy);
                    if(settings.loadRandomNameFromList) DrawPropertyField("unitNamesList");
                }
                EditorGUI.indentLevel--;
            }

            //fov Settings
            if(settings.unitType == UNITTYPE.ENEMY){
                EditorGUILayout.BeginHorizontal();
                if(GUILayout.Button(new GUIContent(space + "Field Of View Settings", GetArrow(foldOutList["fovFoldout"])), FoldOutStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(30))) foldOutList["fovFoldout"] = !foldOutList["fovFoldout"];
                if(GUILayout.Button(new GUIContent("", GetInfoIcon()), FoldOutStyle(), GUILayout.Width(50), GUILayout.Height(30))) showInfo(11, settings.unitType);
                EditorGUILayout.EndHorizontal();
                if(foldOutList["fovFoldout"]) {
                    EditorGUI.indentLevel++;
                    DrawPropertyFields(fovFields);
                    EditorGUI.indentLevel--;
                }
            }
        }

        //visualize AttackData
        void ShowEnemyAttackData(){

            //access serializedProperty enemyAttackList
            SerializedProperty enemyAttackListProperty = serializedObject.FindProperty("enemyAttackList");

            //check if this list has elements
            if(enemyAttackListProperty != null && enemyAttackListProperty.isArray){

                //show message when there are no items
                if(enemyAttackListProperty.arraySize == 0) EditorGUILayout.LabelField("No Enemy Attack Data Available");

                //show list with attack data
                for(int i = 0; i < enemyAttackListProperty.arraySize; i++) {
                    SerializedProperty attackDataProperty = enemyAttackListProperty.GetArrayElementAtIndex(i);
                    ShowAttackData(attackDataProperty, true);
                }

                //footer buttons
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(" ",GUILayout.Width(17));

                //button -
                if(enemyAttackListProperty.arraySize > 0) { //only show + button when there is at least 1 (or more) item(s)
                    if(GUILayout.Button("-", smallButtonStyle())) enemyAttackListProperty.DeleteArrayElementAtIndex(enemyAttackListProperty.arraySize - 1); //remove last item
                }

                //button +
                if(GUILayout.Button("+", smallButtonStyle(), GUILayout.Width(25))) enemyAttackListProperty.InsertArrayElementAtIndex(enemyAttackListProperty.arraySize);

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(10);
            }
        }

        //visualize the combo data
        void ShowComboData(List<Combo> comboList){
            if(comboList.Count == 0) EditorGUILayout.LabelField("No Combo Data Available");

            //create list of combos
            foreach(Combo combo in comboList){

                combo.foldout = EditorGUILayout.Foldout(combo.foldout, combo.comboName, true);
                if(combo.foldout){

                    //name field
                    combo.comboName = EditorGUILayout.TextField("Combo Name:", combo.comboName);
                
                    //attack sequence title
                    ShowHeader("Attack Sequence");

                    //list of attacks
                    if(combo.attackSequence.Count == 0) EditorGUILayout.LabelField("This combo does not have any attacks listed");
                    foreach(AttackData data in combo.attackSequence){
                        EditorGUI.indentLevel++;
                        data.foldout = EditorGUILayout.Foldout(data.foldout, data.name, true);
                        if(data.foldout){
                            data.name = EditorGUILayout.TextField("Attack Name:", data.name);
                            data.damage = EditorGUILayout.IntField("Damage", data.damage);
                            data.sfx = EditorGUILayout.TextField("Sfx (on hit)", data.sfx);
                            data.animationState = EditorGUILayout.TextField("Animation State", data.animationState);
                            data.attackType = (ATTACKTYPE)EditorGUILayout.EnumPopup("Attack Type", data.attackType);
                            data.knockdown = EditorGUILayout.Toggle("Knockdown", data.knockdown);
                            GUILayout.Space(10);
                        }
                        EditorGUI.indentLevel--;
                    }
                
                    //footer buttons
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(" ",GUILayout.Width(17));

                    //button -
                    if(comboList.Count > 0) if(GUILayout.Button("-", smallButtonStyle())) combo.attackSequence.RemoveAt(combo.attackSequence.Count-1);

                    //button +
                    if(GUILayout.Button("+", smallButtonStyle(), GUILayout.Width(25))) combo.attackSequence.Add(new AttackData("[New Attack]", 0, null, ATTACKTYPE.NONE, false));
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(10);
                }
            }
        
            //combo footer buttons
            EditorGUILayout.Space(15);
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("Add Combo", GUILayout.Width(200), GUILayout.Height(25))) comboList.Add(new Combo());
            if(comboList.Count > 0)if(GUILayout.Button("Remove Combo", GUILayout.Width(200), GUILayout.Height(25))) comboList.RemoveAt(comboList.Count-1);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(10);
        }

        //visualize attack data
        void ShowAttackData(SerializedProperty property, bool showName){
            EditorGUI.indentLevel++;

            SerializedProperty foldout = property.FindPropertyRelative("foldout");
            SerializedProperty nameProp = property.FindPropertyRelative("name");

            //set name of foldout item
            if(showName){
                string foldoutLabel = nameProp != null? nameProp.stringValue : ObjectNames.NicifyVariableName(property.name);
                foldout.boolValue = EditorGUILayout.Foldout(foldout.boolValue, new GUIContent(foldoutLabel), true);
            } else {
                foldout.boolValue = EditorGUILayout.Foldout(foldout.boolValue, property.name, true);
            }

             if (foldout.boolValue) {
                SerializedProperty damageProp = property.FindPropertyRelative("damage");
                SerializedProperty animationStateProp = property.FindPropertyRelative("animationState");
                SerializedProperty sfxProp = property.FindPropertyRelative("sfx");
                SerializedProperty attackTypeProp = property.FindPropertyRelative("attackType");
                SerializedProperty knockdownProp = property.FindPropertyRelative("knockdown");

                if(showName) EditorGUILayout.PropertyField(nameProp, new GUIContent("Attack Name:"));
                EditorGUILayout.PropertyField(damageProp, new GUIContent("Damage"));
                EditorGUILayout.PropertyField(animationStateProp, new GUIContent("Animation State"));
                EditorGUILayout.PropertyField(sfxProp, new GUIContent("sfx"));
                EditorGUILayout.PropertyField(attackTypeProp, new GUIContent("Attack Type"));
                EditorGUILayout.PropertyField(knockdownProp, new GUIContent("Knockdown"));
                GUILayout.Space(10);
            }
            EditorGUI.indentLevel--;
        }

        //caches all serialized properties (for multi editing support)
        private void CacheSerializedProperties(){
            var targetType = typeof(UnitSettings);
            var fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            properties = new SerializedProperty[fields.Length];
            for (int i = 0; i < fields.Length; i++) properties[i] = serializedObject.FindProperty(fields[i].Name);   
        }

        //draw a list of property fields
        public void DrawPropertyFields(HashSet<string> propertyHash) {
            foreach (var property in properties) {
                if (property != null && propertyHash.Contains(property.name)) {

                    //check if the property is of type Sprite
                    if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue is Sprite) {

                        //draw the sprite field with a thumbnail icon
                        property.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField(
                            new GUIContent(ObjectNames.NicifyVariableName(property.name)), 
                            property.objectReferenceValue, 
                            typeof(Sprite), 
                            allowSceneObjects: false);

                    } else {

                        //draw the field normally for other types
                        EditorGUILayout.PropertyField(property, new GUIContent(ObjectNames.NicifyVariableName(property.name)));
                    }
                }
            }
        }

        //returns cached property
        public SerializedProperty GetPropertyByName(string propertyName){
            foreach (var property in properties) if(property != null && property.name == propertyName) return property;
            return null;
        }

        //draw one property field
        public void DrawPropertyField(string propertyName){
            DrawPropertyFields( new HashSet<string>{ propertyName } );
        }
         
        //header
        void ShowHeader(string label){
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.wordWrap = true;
            style.richText = true;
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = 13;
            style.richText = true;
            style.padding = new RectOffset(16,0,4,0);
            GUILayout.Label(label, style);
        }

        //GUIStyle for foldout buttons
        GUIStyle FoldOutStyle(){
            bool isDarkMode = EditorGUIUtility.isProSkin; //check if unity editor is set to light or dark mode
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.alignment = TextAnchor.MiddleLeft; //alight left
            style.fixedHeight = 32;
            style.stretchWidth = true;
            style.padding = new RectOffset(12,10,0,0);
            style.margin = new RectOffset(0,0,5,5);
            style.normal.background = MakeTex(1, 1, new Color(1f, 1f, 1f, isDarkMode? 0.1f : 0.2f));  // set the button background color
            style.normal.textColor = isDarkMode? Color.white : Color.black;
            return style;
        }

        //GUIStyle for small + - Buttons
        GUIStyle smallButtonStyle(){
            bool isDarkMode = EditorGUIUtility.isProSkin; //check if unity editor is set to light or dark mode
            GUIStyle style = new GUIStyle(GUI.skin.button);
            style.fixedHeight = 22;
            style.fixedWidth = 22;
            style.fontSize = 18;
            style.padding = new RectOffset(2,2,2,2);
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.background = MakeTex(1, 1, new Color(1f, 1f, 1f, isDarkMode? 0.1f : 0.2f));  // set the button background color
            style.normal.textColor = isDarkMode? Color.white : Color.black;
            return style;
        }

        //creates a background texture for foldout button
        private Texture2D MakeTex(int width, int height, Color color) {
            Color[] pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            Texture2D texture = new Texture2D(width, height);
            texture.SetPixels(pixels);
            texture.Apply();
            return texture;
        }

        //returns a proper arrow icon
        Texture2D GetArrow(bool isFoldedOut) {
            if(iconArrowClose == null || iconArrowOpen == null) return null;
            else return isFoldedOut? iconArrowOpen : iconArrowClose;
        }

        //returns a proper arrow icon
        Texture2D GetInfoIcon() {
            if(iconInfo == null) return null;
            else return iconInfo;
        }

        //shortcut to highlight items
        string highlightItem(string label, int size = 13){
            return "<b><size=" + size + "><color=#FFFFFF>" + label + "</color></size></b>";
        }

        //shows documentation when the user presses the ? icon
        public void showInfo(int id, UNITTYPE unitType){
            string title = "";
            string content = "";

            switch(id) {
            case 0:
                title = "Linked Components";
                content = "This section contains links to several components and external references used by this unit. Below is a description of each item:" + newline;
                content += highlightItem("Shadow Prefab: ") + "A shadow sprite positioned at the base of the unit. (Optional)" + newline;
                content += highlightItem("Weapon Bone: ") + "A transform that represents the position of the unit's hand. When a weapon is picked up, it will be parented to the weapon bone." + newline;
                content += highlightItem("Hit Effect: ") + "An effect displayed when the unit is hit. (Optional)" + newline;
                content += highlightItem("Hitbox: ") + "A link to a sprite (red box) representing the hit area during an attack animation." + newline;
                content += highlightItem("Sprite Renderer: ") + "A Link to the sprite of this unit." + newline;
                break;

            case 1:
                title = "Movement Settings";
                content = "These values define how fast a unit moves around a level. Here’s what each term means:" + newline;
                content += highlightItem("Start Direction: ") + "The direction the unit faces at the beginning of a level." + newline;
                content += highlightItem("Move Speed: ") + "The unit's running speed when moving across the level." + newline;
                content += highlightItem("Move Speed Air: ") + "The unit’s speed while in the air during a jump." + newline;
                content += highlightItem("Use Acceleration: ") + "Option to enable or disable gradual speed changes." + newline;
                content += highlightItem("Move Acceleration: ") + "The rate at which the unit's speed increases when accelerating." + newline;
                content += highlightItem("Move Deceleration: ") + "The rate at which the unit's speed decreases when slowing down." + newline;
                break;

            case 2:
                title = "Jump Settings";
                content = "These values define the jump behaviour of a unit." + newline;
                content += highlightItem("Jump Height: ") + "The height of a jump" + newline;
                content += highlightItem("Jump Speed: ") + "The speed of the jump simulation" + newline;
                content += highlightItem("Gravity: ") + "The strength of gravitational force applied to the character during a jump." + newline;
                break;

            case 3:
                title = "Attack Data";
                content = "This section provides a list of attack details, where you can modify data such as damage, animation, attack type, and other data for each attack." + newline;
                content += highlightItem("Damage: ") + "The amount of Health Points subtracted from the enemy's health bar." + newline;
                content += highlightItem("Animation State: ") + "The animation state that needs to be played on this unit's Animator component." + newline;
                content += highlightItem("Attack Type: ") + "The attack type of this attack." + newline;
                content += highlightItem("Knockdown: ") + "Indicates if a successful hit causes the enemy to be knocked down." + newline;
                break;

            case 4:
                title = "Combo Settings";
                content = "The combo section allows you to configure and manage sequential attacks. Here, you can set up a series of moves that will be executed in a specific order, creating a combo." + newline;
                content += highlightItem("Combo Reset Time: ") + "If the player presses a button within this time window, it will count as part of the combo sequence." + newline;
                content += highlightItem("Continue Combo On Hit: ") + "Option to only proceed with the combo if the attack connects; otherwise, restart the combo sequence." + newline;
                content += highlightItem("Attack Sequence") + "\n";

                content += "For each combo attack you can modify data such as damage, animation, attack type:" + newline;
                content += highlightItem("Attack Name: ") + "The name of this attack." + newline;
                content += highlightItem("Damage: ") + "The amount of Health Points subtracted from the enemy's health bar." + newline;
                content += highlightItem("Animation State: ") + "The animation state that needs to be played on this unit's Animator component." + newline;
                content += highlightItem("Attack Type: ") + "The attack type of this attack." + newline;
                content += highlightItem("Knockdown: ") + "Indicates if a successful hit causes the enemy to be knocked down." + newline;
                break;

            case 5:
                title = "Knockdown Settings";
                content = "These values determine how a unit behaves when knocked down:" + newline;
                content += highlightItem("Can Be Knocked Down: ") + "Whether or not this unit can be knocked down." + newline;
                content += highlightItem("Knockdown Height: ") + "The height to which a unit is propelled upward when knocked down." + newline;
                content += highlightItem("Knockdown Distance: ") + "The distance a unit is pushed backward during a knockdown." + newline;
                content += highlightItem("Knockdown Speed: ") + "The speed of the Knockdown simulation." + newline;
                content += highlightItem("Knockdown Floor Time: ") + "The duration a unit remains on the ground before getting back up." + newline;
                break;

            case 6:
                title = "Throw Settings";
                content = "These values determine how a unit behaves when being thrown:" + newline;
                content += highlightItem("Throw Height: ") + "The height to which a unit is propelled upward when being thrown." + newline;
                content += highlightItem("Throw Distance: ") + "The distance a unit travels while in the air after being thrown." + newline;
                break;

            case 7:
                title = "Defence Settings";
                content = "These values determine how a unit behaves while defending:" + newline;
                if(unitType == UNITTYPE.PLAYER) content += highlightItem("Can Change Dir While Defending: ") + "Enable or disable the ability for the player to change direction while holding the defence button." + newline;
                if(unitType == UNITTYPE.ENEMY) content += highlightItem("Defend Chance: ") + "The probability (0 - 100) that the enemy will successfully defend against an incoming attack." + newline;
                if(unitType == UNITTYPE.ENEMY) content += highlightItem("Defend Duration: ") + "The amount of time an enemy remains in the defence state after initiating defense." + newline;
                content += highlightItem("Rear Defense Enabled: ") + "Determines whether this unit can defend against attacks coming from behind." + newline;   
                break;

            case 8:
                title = "Grab Settings";
                content = "These values determine how a player behaves when grabbing and holding an enemy:" + newline;
                content += highlightItem("Grab Animation: ") + "The name of the animation state that contains the Grab Animation in this unit's Animator component" + newline;
                content += highlightItem("Grab Position: ") + "The position this unit moves to while grabbing, relative to the enemy it is holding." + newline;
                content += highlightItem("Grab Duration: ") + "The duration of the grab, before this unit and it's target return back to normal." + newline;
                break;

            case 9:
                title = "Weapon Settings";
                content = "\n";
                content += highlightItem("Lose Weapon When Hit: ") + "Specifies whether the unit should drop the currently equipped weapon when hit." + newline;
                content += highlightItem("Lose Weapon When Knocked Down: ") + "Determines whether the unit retains or drops the currently equipped weapon when knocked down." + newline;
                break;

            case 10:
                title = "Unit Name & Portrait";
                content = "\n";
                if(unitType == UNITTYPE.ENEMY) content += highlightItem("Load Random Name From List: ") + "Option to load a random enemy name from a txt file." + newline;
                if(unitType == UNITTYPE.PLAYER) content += highlightItem("Unit Name: ") + "The unit's name as shown in the top left corner near the health bar." + newline;
                if(unitType == UNITTYPE.ENEMY) content += highlightItem("Unit Name: ") + "The unit's name as shown in the top near the enemy health bar." + newline;
                content += highlightItem("Show name in all caps: ") + "Determines whether the name should be displayed in capital letters." + newline;
                content += highlightItem("Unit Portrait: ") + "The unit portrait sprite is a small icon displayed at the top, near the health bar." + newline;

                break;
            case 11:
                title = "Field Of View Settings";
                content = "These values determine whether an enemy detects the player when they enter its Field Of View (FOV)." + newline;
                content += highlightItem("Enable FOV: ") + "Enable or disable the Field Of View. When disabled a unit always spots the player by default." + newline;
                content += highlightItem("View Distance: ") + "How far this unit can see." + newline;
                content += highlightItem("View Angle: ") + "How wide this unit can see." + newline;
                content += highlightItem("View position Offset: ") + "The starting position (eye level) of the view cone, " + newline;
                content += highlightItem("Show FOV Cone in Editor: ") + "Useful for debugging, this option displays the field of view cone in the Unity Editor." + newline;
                content += highlightItem("Target in Sight: ") + "A read-only value for debugging that indicates whether the target has been spotted." + newline;
                break;

            }
            CustomWindow.ShowWindow(title, content, new Vector2(600, 500));
        }
    }
}