using System.Collections.Generic;
using Assets.WorldObject.Building;
using UnityEngine;

namespace Assets.Player.HUD
{
    public class Hud : MonoBehaviour {
        public GUISkin ResourceSkin, OrdersSkin, SelectBoxSkin;
        private Player _player;
        private const int OrdersBarWidth = 150, ResourceBarHeight = 40;
        private const int SelectionNameHeight = 30;

        public Texture2D ActiveCursor;
        public Texture2D SelectCursor, LeftCursor, RightCursor, UpCursor, DownCursor;
        public Texture2D[] MoveCursors, AttackCursors, HarvestCursors;
        public GUISkin MouseCursorSkin;
        private CursorState _activeCursorState;
        private int _currentFrame = 0;
        private Dictionary<ResourceType, int> _resourceValues, _resourceLimits;
        private const int IconWidth = 32, IconHeight = 32, TextWidth = 128, TextHeight = 32;

        public Texture2D[] Resources;
        private Dictionary<ResourceType, Texture2D> _resourceImages;

        private WorldObject.WorldObject _lastSelection;
        private float _sliderValue;
        public Texture2D ButtonHover, ButtonClick;
        private const int BuildImageWidth = 64, BuildImageHeight = 64;
        private int _buildAreaHeight = 0;
        private const int ButtonSpacing = 7;
        private const int ScrollBarWidth = 22;
        private const int BuildImagePadding = 8;
        public Texture2D BuildFrame, BuildMask;
        public Texture2D SmallButtonHover, SmallButtonClick;
        public Texture2D RallyPointCursor;
        private CursorState _previousCursorState;
 


        // Use this for initialization
        void Start () {
            _player = transform.root.GetComponent<Player>();
            ResourceManager.StoreSelectBoxItems(SelectBoxSkin);
            SetCursorState(CursorState.Select);

            _buildAreaHeight = Screen.height - ResourceBarHeight - SelectionNameHeight - 2 * ButtonSpacing;

            //resource images
            _resourceValues = new Dictionary<ResourceType, int>();
            _resourceLimits = new Dictionary<ResourceType, int>();

            _resourceImages = new Dictionary<ResourceType, Texture2D>();
            for (int i = 0; i < Resources.Length; i++)
            {
                switch (Resources[i].name)
                {
                    case "Money":
                        _resourceImages.Add(ResourceType.Money, Resources[i]);
                        Debug.Log("money pic added");
                        _resourceValues.Add(ResourceType.Money, 0);
                        _resourceLimits.Add(ResourceType.Money, 0);
                        break;
                    case "Power":
                        _resourceImages.Add(ResourceType.Power, Resources[i]);
                        _resourceValues.Add(ResourceType.Power, 0);
                        _resourceLimits.Add(ResourceType.Power, 0);
                        break;
                    default: break;
                }
            }

        }
    
        // Update is called once per frame
        void OnGui () {

            if (_player && _player.Human)
            {
                DrawOrdersBar();
                DrawResourceBar();
                //Debug.Log ("hud drawing called");
                DrawMouseCursor();
            }

        }

        private void DrawOrdersBar()
        {
            GUI.skin = OrdersSkin;
            GUI.BeginGroup(new Rect(Screen.width - OrdersBarWidth - BuildImageWidth, ResourceBarHeight, OrdersBarWidth + BuildImageWidth, Screen.height - ResourceBarHeight));
            GUI.Box(new Rect(BuildImageWidth + ScrollBarWidth, 0, OrdersBarWidth, Screen.height - ResourceBarHeight), "");
            string selectionName = "";
            if (_player.SelectedObject)
            {
                selectionName = _player.SelectedObject.ObjectName;
                if (_player.SelectedObject.IsOwnedBy(_player))
                {
                    //reset slider value if the selected object has changed
                    if (_lastSelection && _lastSelection != _player.SelectedObject) _sliderValue = 0.0f;
                    DrawOperations(_player.SelectedObject.GetOperations());
                    //store the current selection
                    _lastSelection = _player.SelectedObject;

                    Building selectedBuilding = _lastSelection.GetComponent<Building>();
                    if (selectedBuilding)
                    {
                        DrawBuildQueue(selectedBuilding.GetBuildQueueValues(), selectedBuilding.GetBuildPercentage());
                        DrawStandardBuildingOptions(selectedBuilding);
                    }
                }
            }
            if (!selectionName.Equals(""))
            {
                int topPos = _buildAreaHeight + ButtonSpacing;
                GUI.Label(new Rect(0, topPos, OrdersBarWidth, SelectionNameHeight), selectionName);
            }



            GUI.EndGroup();
        }

        private void DrawResourceBar()
        {
            GUI.skin = ResourceSkin;
            GUI.BeginGroup(new Rect(0, 0, Screen.width, ResourceBarHeight));
            GUI.Box(new Rect(0, 0, Screen.width, ResourceBarHeight), "");

            //Draw resources
            int topPos = 4, iconLeft = 4, textLeft = 35;
            DrawResourceIcon(ResourceType.Money, iconLeft, textLeft, topPos);
            iconLeft += TextWidth;
            textLeft += TextWidth;
            DrawResourceIcon(ResourceType.Power, iconLeft, textLeft, topPos);


            GUI.EndGroup();
        }

        private void DrawResourceIcon(ResourceType type, int iconLeft, int textLeft, int topPos)
        {
        
            Texture2D icon = _resourceImages[type];
            string text = _resourceValues[type].ToString() + "/" + _resourceLimits[type].ToString();
            GUI.DrawTexture(new Rect(iconLeft, topPos, IconWidth, IconHeight), icon);
            GUI.Label(new Rect(textLeft, topPos, TextWidth, TextHeight), text);
        }

        public bool MouseInBounds
        {
            get
            {
                //Screen coordinates start in the lower-left corner of the screen
                //not the top-left of the screen like the drawing coordinates do
                Vector3 mousePos = Input.mousePosition;
                bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width - OrdersBarWidth;
                bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height - ResourceBarHeight;

                return insideWidth && insideHeight;
            }
        }

        public Rect GetPlayingArea()
        {
            return new Rect(0, ResourceBarHeight, Screen.width - OrdersBarWidth, Screen.height - ResourceBarHeight);
        }

        private void DrawMouseCursor()
        {
            bool mouseOverHud = !MouseInBounds && _activeCursorState != CursorState.PanRight && _activeCursorState != CursorState.PanUp;

            if (!mouseOverHud)
            {   
                Cursor.visible = true;
                GUI.skin = MouseCursorSkin;
                GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
                UpdateCursorAnimation();
                Rect cursorPosition = GetCursorDrawPosition();
                GUI.Label(cursorPosition, ActiveCursor);
                GUI.EndGroup();
            }
            else
            {
                Cursor.visible = false;
            }

        
        }

        private void UpdateCursorAnimation()
        {
            //sequence animation for cursor (based on more than one image for the cursor)
            //change once per second, loops through array of images
            if (_activeCursorState == CursorState.Move)
            {
                _currentFrame = (int)Time.time % MoveCursors.Length;
                ActiveCursor = MoveCursors[_currentFrame];
            }
            else if (_activeCursorState == CursorState.Attack)
            {
                _currentFrame = (int)Time.time % AttackCursors.Length;
                ActiveCursor = AttackCursors[_currentFrame];
            }
            else if (_activeCursorState == CursorState.Harvest)
            {
                _currentFrame = (int)Time.time % HarvestCursors.Length;
                ActiveCursor = HarvestCursors[_currentFrame];
            }
        }

        private Rect GetCursorDrawPosition()
        {
            //set base position for custom cursor image
            float leftPos = Input.mousePosition.x;
            float topPos = Screen.height - Input.mousePosition.y; //screen draw coordinates are inverted
            //adjust position base on the type of cursor being shown
            if (_activeCursorState == CursorState.PanRight) leftPos = Screen.width - ActiveCursor.width;
            else if (_activeCursorState == CursorState.PanDown) topPos = Screen.height - ActiveCursor.height;
            else if (_activeCursorState == CursorState.Move || _activeCursorState == CursorState.Select || _activeCursorState == CursorState.Harvest)
            {
                topPos -= ActiveCursor.height / 2;
                leftPos -= ActiveCursor.width / 2;
            }

            else if (_activeCursorState == CursorState.RallyPoint) topPos -= ActiveCursor.height;


            return new Rect(leftPos, topPos, ActiveCursor.width, ActiveCursor.height);
        }


        public void SetCursorState(CursorState newState)
        {
            if (_activeCursorState != newState) _previousCursorState = _activeCursorState;
            _activeCursorState = newState;
            switch (newState)
            {
                case CursorState.Select:
                    ActiveCursor = SelectCursor;
                    break;
                case CursorState.Attack:
                    _currentFrame = (int)Time.time % AttackCursors.Length;
                    ActiveCursor = AttackCursors[_currentFrame];
                    break;
                case CursorState.Harvest:
                    _currentFrame = (int)Time.time % HarvestCursors.Length;
                    ActiveCursor = HarvestCursors[_currentFrame];
                    break;
                case CursorState.Move:
                    Debug.Log("set cursor move");
                    _currentFrame = (int)Time.time % MoveCursors.Length;
                    ActiveCursor = MoveCursors[_currentFrame];
                    break;
                case CursorState.PanLeft:
                    ActiveCursor = LeftCursor;
                    break;
                case CursorState.PanRight:
                    ActiveCursor = RightCursor;
                    break;
                case CursorState.PanUp:
                    ActiveCursor = UpCursor;
                    break;
                case CursorState.PanDown:
                    ActiveCursor = DownCursor;
                    break;
                default: break;
                case CursorState.RallyPoint:
                    ActiveCursor = RallyPointCursor;
                    break;
            }
        }

        public void SetResourceValues(Dictionary<ResourceType, int> resourceValues, Dictionary<ResourceType, int> resourceLimits)
        {
            this._resourceValues = resourceValues;
            this._resourceLimits = resourceLimits;
        }


        private void DrawOperations(string[] operations)
        {
            GUIStyle buttons = new GUIStyle();
            buttons.hover.background = ButtonHover;
            buttons.active.background = ButtonClick;
            GUI.skin.button = buttons;
            int numOperations = operations.Length;
            //define the area to draw the operations inside
            GUI.BeginGroup(new Rect(BuildImageWidth, 0, OrdersBarWidth, _buildAreaHeight));
            //draw scroll bar for the list of operations if need be
            if (numOperations >= MaxNumRows(_buildAreaHeight)) DrawSlider(_buildAreaHeight, numOperations / 2.0f);
            //display possible operations as buttons and handle the button click for each
            for (int i = 0; i < numOperations; i++)
            {
                int column = i % 2;
                int row = i / 2;
                Rect pos = GetButtonPos(row, column);
                Texture2D action = ResourceManager.GetBuildImage(operations[i]);
                if (action)
                {
                    //create the button and handle the click of that button
                    if (GUI.Button(pos, action))
                    {
                        if (_player.SelectedObject) _player.SelectedObject.PerformOperation(operations[i]);
                    }
                }
            }
            GUI.EndGroup();
        }

        private int MaxNumRows(int areaHeight)
        {
            return areaHeight / BuildImageHeight;
        }

        private Rect GetButtonPos(int row, int column)
        {
            int left = ScrollBarWidth + column * BuildImageWidth;
            float top = row * BuildImageHeight - _sliderValue * BuildImageHeight;
            return new Rect(left, top, BuildImageWidth, BuildImageHeight);
        }

        private void DrawSlider(int groupHeight, float numRows)
        {
            //slider goes from 0 to the number of rows that do not fit on screen
            _sliderValue = GUI.VerticalSlider(GetScrollPos(groupHeight), _sliderValue, 0.0f, numRows - MaxNumRows(groupHeight));
        }

        private Rect GetScrollPos(int groupHeight)
        {
            return new Rect(ButtonSpacing, ButtonSpacing, ScrollBarWidth, groupHeight - 2 * ButtonSpacing);
        }

        private void DrawBuildQueue(string[] buildQueue, float buildPercentage)
        {
            for (int i = 0; i < buildQueue.Length; i++)
            {
                float topPos = i * BuildImageHeight - (i + 1) * BuildImagePadding;
                Rect buildPos = new Rect(BuildImagePadding, topPos, BuildImageWidth, BuildImageHeight);
                GUI.DrawTexture(buildPos, ResourceManager.GetBuildImage(buildQueue[i]));
                GUI.DrawTexture(buildPos, BuildFrame);
                topPos += BuildImagePadding;
                float width = BuildImageWidth - 2 * BuildImagePadding;
                float height = BuildImageHeight - 2 * BuildImagePadding;
                if (i == 0)
                {
                    //shrink the build mask on the item currently being built to give an idea of progress
                    topPos += height * buildPercentage;
                    height *= (1 - buildPercentage);
                }
                GUI.DrawTexture(new Rect(2 * BuildImagePadding, topPos, width, height), BuildMask);
            }
        }


        private void DrawStandardBuildingOptions(Building building)
        {
            GUIStyle buttons = new GUIStyle();
            buttons.hover.background = SmallButtonHover;
            buttons.active.background = SmallButtonClick;
            GUI.skin.button = buttons;
            int leftPos = BuildImageWidth + ScrollBarWidth + ButtonSpacing;
            int topPos = _buildAreaHeight - BuildImageHeight / 2;
            int width = BuildImageWidth / 2;
            int height = BuildImageHeight / 2;

            if (GUI.Button(new Rect(leftPos, topPos, width, height), building.SellImage))
            {

                Debug.Log("sell button triggered");
                building.Sell();
            }

            leftPos += width + ButtonSpacing;


            if (building.HasSpawnPoint())
            {
                if (GUI.Button(new Rect(leftPos, topPos, width, height), building.RallyPointImage))
                {

                    if (_activeCursorState != CursorState.RallyPoint && _previousCursorState != CursorState.RallyPoint) SetCursorState(CursorState.RallyPoint);
                    else {
                        //dirty hack to ensure toggle between RallyPoint and not works ...
                        SetCursorState(CursorState.PanRight);
                        SetCursorState(CursorState.Select);
                    }
                }


            }

   
        }

        public CursorState GetPreviousCursorState()
        {
            return _previousCursorState;
        }

        public CursorState GetCursorState()
        {
            return _activeCursorState;
        }

  
    }
}
