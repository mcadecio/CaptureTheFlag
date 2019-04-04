using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using unityChan.Characters.ThirdPerson;


    public class ThirdPersonInput : MonoBehaviour
    {

        public FixedJoystick LeftJoystick;
        public FixedJoystick RightJoystick;
        public FixedButton JumpButton;
        public FixedButton CrouchButton;
        //public FixedTouchField TouchField;

        protected ThirdPersonUserControl Control;

        protected float CameraAngle;
        protected float CameraAngleSpeed = 2f;

        // Use this for initialization
        void Start()
        {
            Control = GetComponent<ThirdPersonUserControl>();

        }

        // Update is called once per frame
        void Update()
        {
            Control.m_Jump = JumpButton.Pressed;
            Control.m_Crouch = CrouchButton.Pressed;
            Control.Hinput = LeftJoystick.inputVector.x;
            Control.Vinput = LeftJoystick.inputVector.y;

        //CameraAngle += TouchField.TouchDist.x * CameraAngleSpeed;
        CameraAngle += RightJoystick.inputVector.x * CameraAngleSpeed;

        Camera.main.transform.position = transform.position + Quaternion.AngleAxis(CameraAngle, Vector3.up) * new Vector3(0, 3, 4);
        Camera.main.transform.rotation = Quaternion.LookRotation(transform.position + Vector3.up * 2f - Camera.main.transform.position, Vector3.up);

        }
    }
