using System;
using System.Collections.Generic;
using System.Linq;

namespace DefenceGameSystem.OS.Kernel
{
    public static class InputModule
    {
        private readonly static Dictionary<int, IInputableObject> s_m_buttons;
        private readonly static Dictionary<string, IInputableObject[]> s_m_axises;

        private readonly static Dictionary<int, bool> s_m_isPressing;
        private readonly static Dictionary<int, bool> s_m_isPressedDown;
        private readonly static Dictionary<int, bool> s_m_isPressedUp;

        private readonly static List<int> s_m_tempInts;

        static InputModule()
        {
            s_m_buttons = new Dictionary<int, IInputableObject>();
            s_m_axises = new Dictionary<string, IInputableObject[]>();

            s_m_isPressing = new Dictionary<int, bool>();
            s_m_isPressedDown = new Dictionary<int, bool>();
            s_m_isPressedUp = new Dictionary<int, bool>();

            s_m_tempInts = new List<int>();
        }

        // 입력 가능한 개체를 키에 등록합니다.
        // 하나의 키에는 입력 가능한 개체를 단 한 개만 등록할 수 있습니다.
        public static bool AddButton(IInputableObject button, KeyType type)
        {
            // 지정되지 않은 버튼인 경우
            if(button == default(IInputableObject))
                throw new ArgumentNullException("button", "parameter cannot be null.");
            // 지정되지 않은 키 타입인 경우
            if(type == default(KeyType))
                throw new ArgumentNullException("type", "parameter cannot be null.");
            // 버튼 테이블에 이미 등록된 버튼인 경우
            if(s_m_buttons.ContainsValue(button))
                throw new AlreadyExistObjectException("button was already registered.");
            // 등록하고자 하는 키에 이미 다른 버튼이 등록된 경우
            if(s_m_buttons.ContainsKey((int)type) && s_m_buttons[(int)type] != default(IInputableObject))
                throw new AlreadyExistObjectException("this key type was already allocated by other button.");

            s_m_buttons.Add((int)type, button);
            s_m_isPressing.Add((int)type, false);
            s_m_isPressedDown.Add((int)type, false);
            s_m_isPressedUp.Add((int)type, false);

            return true;
        }

        // 키에 등록된 입력 가능한 개체를 해제합니다.
        public static bool RemoveButton(KeyType type)
        {
            // 지정되지 않은 키 타입인 경우
            if(type == default(KeyType))
                throw new ArgumentNullException("type", "parameter cannot be null.");
            // 버튼 테이블에 등록되지 않은 키 타입인 경우
            if(!s_m_buttons.ContainsKey((int)type))
                throw new ArgumentOutOfRangeException("type", "this key type was not registered.");

            s_m_buttons.Remove((int)type);
            s_m_isPressing.Remove((int)type);
            s_m_isPressedDown.Remove((int)type);
            s_m_isPressedUp.Remove((int)type);

            return true;
        }

        // 입력 가능한 개체에 방향성을 부여합니다.
        // 키 타입이 할당된 버튼만 등록할 수 있습니다.
        public static bool AddAxis(IInputableObject button, string axisName, AxisType axis)
        {
            // 지정되지 않은 버튼인 경우
            if(button == default(IInputableObject))
                throw new ArgumentNullException("button", "parameter cannot be null.");
            // axis 이름이 지정되지 않거나, 빈 문자열일 경우
            if(axisName == null || axisName == "")
                throw new ArgumentException("axisName", "parameter cannot be null or empty string.");
            // 지정되지 않은 키 타입인 경우
            if(axis == default(AxisType))
                throw new ArgumentNullException("axis", "parameter cannot be null.");
            // 버튼 테이블에 등록되어 있지 않은 버튼인 경우
            if(!s_m_buttons.ContainsValue(button))
                throw new ArgumentOutOfRangeException("button", "this button was not registered.");

            int axisIndex;

            axisIndex = s_m_Axis2Index(axis);

            if(s_m_axises.ContainsKey(axisName))
            {
                // 현 위치에 이미 등록된 버튼이 있는 경우
                if(s_m_axises[axisName][axisIndex] != null)
                    throw new AlreadyExistObjectException("this axis type was already allocated by other button.");
                // 서로 다른 axis에 대해 같은 버튼을 사용하려는 경우
                if(s_m_axises[axisName][1 - axisIndex] == button)
                    throw new AlreadyExistObjectException("this button was already allocated on opposite axis type in same axis name.");

                s_m_axises[axisName][axisIndex] = button;

                return true;
            }
            else
            {
                s_m_axises.Add(axisName, new IInputableObject[2]);

                s_m_axises[axisName][axisIndex] = button;

                return true;
            }
        }

        // 방향 유형을 제거합니다.
        // 해당 방향 유형에 등록되어 있던 버튼의 방향성이 상실됩니다.
        public static bool RemoveAxis(string axisName)
        {
            // axis 이름이 지정되지 않거나, 빈 문자열일 경우
            if(axisName == null || axisName == "")
                throw new ArgumentException("axisName", "parameter cannot be null or empty string.");
            // 존재하지 않는 axis 이름일 경우
            if(!s_m_axises.ContainsKey(axisName))
                throw new NotExistObjectException("this axis name was not exist.");

            s_m_axises.Remove(axisName);

            return true;
        }

        // 입력 가능한 개체의 방향성을 제거합니다.
        public static bool RemoveAxis(string axisName, IInputableObject button)
        {
            // axis 이름이 지정되지 않거나, 빈 문자열일 경우
            if(axisName == null || axisName == "")
                throw new ArgumentException("axisName", "parameter cannot be null or empty string.");
            // 지정되지 않은 버튼인 경우
            if(button == default(IInputableObject))
                throw new ArgumentNullException("button", "parameter cannot be null.");
            // 버튼 테이블에 등록되어 있지 않은 버튼인 경우
            if(!s_m_buttons.ContainsValue(button))
                throw new ArgumentOutOfRangeException("button", "this button was not registered.");

            if(s_m_axises[axisName][0] == button)
            {
                s_m_axises[axisName][0] = null;
                return true;
            }
            else if(s_m_axises[axisName][1] == button)
            {
                s_m_axises[axisName][1] = null;
                return true;
            }
            // 버튼이 어떤 axis에도 존재하지 않는 경우
            else
            {
                throw new NotExistObjectException("this button was not exist in this axis name.");
            }
        }



        // 키 하나를 누름 상태로 변환합니다.
        public static bool SetKeyDown(KeyType type)
        {
            // 지정되지 않은 키 타입인 경우
            if(type == default(KeyType))
                throw new ArgumentNullException("type", "parameter cannot be null.");
            // 버튼 테이블에 등록되지 않은 키 타입인 경우
            if(!s_m_buttons.ContainsKey((int)type))
                return false;

            s_m_isPressedDown[(int)type] = true;
            s_m_isPressing[(int)type] = true;

            return true;
        }

        // 키 하나를 뗌 상태로 변환합니다.
        public static bool SetKeyUp(KeyType type)
        {
            // 지정되지 않은 키 타입인 경우
            if(type == default(KeyType))
                throw new ArgumentNullException("type", "parameter cannot be null.");
            // 버튼 테이블에 등록되지 않은 키 타입인 경우
            if(!s_m_buttons.ContainsKey((int)type))
                return false;

            s_m_isPressing[(int)type] = false;
            s_m_isPressedUp[(int)type] = true;

            return true;
        }

        // 키 하나가 현재 프레임에 눌렸는지를 알려줍니다.
        public static bool GetKeyDown(KeyType type)
        {
            // 지정되지 않은 키 타입인 경우
            if(type == default(KeyType))
                throw new ArgumentNullException("type", "parameter cannot be null.");
            // 버튼 테이블에 등록되지 않은 키 타입인 경우
            if(!s_m_buttons.ContainsKey((int)type))
                return false;

            return s_m_isPressedDown[(int)type];
        }

        // 키 하나를 현재 프레임에 뗐는지를 알려줍니다.
        public static bool GetKeyUp(KeyType type)
        {
            // 지정되지 않은 키 타입인 경우
            if(type == default(KeyType))
                throw new ArgumentNullException("type", "parameter cannot be null.");
            // 버튼 테이블에 등록되지 않은 키 타입인 경우
            if(!s_m_buttons.ContainsKey((int)type))
                return false;

            return s_m_isPressedUp[(int)type];
        }

        // 키 하나가 지속적으로 눌려지고 있는지를 알려줍니다.
        public static bool GetKey(KeyType type)
        {
            // 지정되지 않은 키 타입인 경우
            if(type == default(KeyType))
                throw new ArgumentNullException("type", "parameter cannot be null.");
            // 버튼 테이블에 등록되지 않은 키 타입인 경우
            if(!s_m_buttons.ContainsKey((int)type))
                return false;

            return s_m_isPressing[(int)type];
        }



        // 해당 axis의 활성 방향을 알려줍니다.
        public static int GetAxis(string axisName)
        {
            // axis 이름이 지정되지 않거나, 빈 문자열일 경우
            if(axisName == null || axisName == "")
                throw new ArgumentException("axisName", "parameter cannot be null or empty string.");
            // 존재하지 않는 axis 이름일 경우
            if(!s_m_axises.ContainsKey(axisName))
                throw new NotExistObjectException("this axis name was not exist.");

            IInputableObject negativeButton, positiveButton;
            int negativeKey, positiveKey;
            int negativeAxis, positiveAxis;

            negativeButton = s_m_axises[axisName][0];
            positiveButton = s_m_axises[axisName][1];

            negativeKey = -1;
            positiveKey = -1;

            negativeAxis = 0;
            positiveAxis = 0;

            if(negativeButton != default(IInputableObject))
                negativeKey = s_m_buttons.FirstOrDefault(x => x.Value.Equals(negativeButton)).Key;
            if(positiveButton != default(IInputableObject))
                positiveKey = s_m_buttons.FirstOrDefault(x => x.Value.Equals(positiveButton)).Key;

            negativeAxis = negativeKey == -1 ? 0 : (s_m_isPressing[negativeKey] ? -1 : 0);
            positiveAxis = positiveKey == -1 ? 0 : (s_m_isPressing[positiveKey] ? 1 : 0);

            return negativeAxis + positiveAxis;
        }


        // TODO: Update Frame의 맨 마지막에 이 함수를 집어넣어야 한다.
        public static void Update()
        {
            s_m_tempInts.Clear();
            foreach(int i in s_m_isPressedDown.Keys) s_m_tempInts.Add(i);
            foreach(int i in s_m_tempInts) s_m_isPressedDown[i] = false;

            s_m_tempInts.Clear();
            foreach(int i in s_m_isPressedUp.Keys) s_m_tempInts.Add(i);
            foreach(int i in s_m_tempInts) s_m_isPressedUp[i] = false;
        }

        private static int s_m_Axis2Index(AxisType axis)
        {
            return (~(int)axis >> 1) & 1;
        }
    }
}