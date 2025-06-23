using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Humelo
{
    ///@brief base class for implementing demo scene.
    public class LavstarTTSDemoBase : MonoBehaviour
    {
        public InputField contentInputField = null; ///< As the name implies

        public Dropdown voiceActorsDropdown = null; ///< As the name implies

        public Dropdown voicesGenderDropdown = null; ///< As the name implies

        public Dropdown targetModelDropdown = null; ///< As the name implies

        public Button playbackButton = null; ///< As the name implies

        public Button stopButton = null; ///< As the name implies

        public AudioSource audioSourceUi = null; ///< As the name implies

        public AudioSource audioSourceHuman = null; ///< As the name implies

        public AudioSource audioSourceSloth = null; ///< As the name implies

        public Text statusText = null; ///< As the name implies

        public LavstarTTSSolutionBase ttsSolution = null; ///< As the name implies

        protected AudioSource audioSource = null; ///< As the name implies

        protected virtual void Start()
        {
            voiceActorsDropdown.onValueChanged.AddListener(VoiceActorsSelectedDropdownOnChangedHandler);
            voicesGenderDropdown.onValueChanged.AddListener(VoiceGenderSelectedDropdownOnChangedHandler);
            targetModelDropdown.onValueChanged.AddListener(TargetModelSelectedDropdownOnChangedHandler);
            playbackButton.onClick.AddListener(PlaybackButtonOnClickHandler);
            stopButton.onClick.AddListener(StopButtonOnClickHandler);

            audioSource = audioSourceUi;
        }

        /*!
        * @brief A function for handling StopButtonOnClick.
        * @return void
        */
        protected virtual void StopButtonOnClickHandler()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
            }
        }

        /*!
        * @brief A function for handling PlaybackButtonOnClick.
        * @return void
        */
        protected virtual void PlaybackButtonOnClickHandler()
        {
            throw new NotImplementedException();
        }

        /*!
        * @brief A function for handling VoiceGenderSelectedDropdownOnChanged.
        * @param[in] index dropdown item number is sent.
        * @return void
        */
        protected virtual void VoiceGenderSelectedDropdownOnChangedHandler(int index)
        {
            throw new NotImplementedException();
        }

        /*!
        * @brief A function for handling VoiceActorsSelectedDropdownOnChanged.
        * @param[in] index dropdown item number is sent.
        * @return void
        */
        protected virtual void VoiceActorsSelectedDropdownOnChangedHandler(int index)
        {
            throw new NotImplementedException();
        }

        /*!
        * @brief A function for handling TargetModelSelectedDropdownOnChanged.
        * @param[in] index dropdown item number is sent.
        * @return void
        */
        private void TargetModelSelectedDropdownOnChangedHandler(int index)
        {
            if (index == 0)
            {
                audioSource = audioSourceUi;
                return;
            }

            if (index == 1)
            {
                audioSource = audioSourceHuman;
                return;
            }

            if (index == 2)
            {
                audioSource = audioSourceSloth;
                return;
            }
        }

        /*!
        * @brief A function to save voice list.
        * @param[in] elements List<string> for fill voiceActorsDropdown ui.
        * @return void
        */
        protected void FillVoicesList(List<string> elements)
        {
            voiceActorsDropdown.ClearOptions();
            voiceActorsDropdown.AddOptions(elements);

            if (elements.Count > 0)
            {
                voiceActorsDropdown.value = 0;
                VoiceActorsSelectedDropdownOnChangedHandler(0);
            }
        }
    }
}
// © 2019-2020 Humelo Inc.