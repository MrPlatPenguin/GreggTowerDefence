using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootstepAudio : MonoBehaviour
{
    [SerializeField] CharacterController characterController;
    [SerializeField] float frequency = 0.5f;
    float lastPlay = 0;
    [SerializeField] SoundClip[] grassStepSounds;
    [SerializeField] SoundClip[] sandStepSounds;
    [SerializeField] SoundClip[] stoneStepSounds;
    bool leftStep;

    // Update is called once per frame
    void Update()
    {
        lastPlay += Time.deltaTime;
        if (characterController.isMoving && lastPlay > (frequency/CharacterController.CurrentShoes.Speed))
        {
            SoundManager.PlaySound(GetBiomeFootstepSound(), transform, true);
            leftStep = !leftStep;
            lastPlay = 0;
        }
    }

    SoundClip GetBiomeFootstepSound()
    {
        int index = leftStep ? 0 : 1;
        Biome currentBiome = characterController.GetCurrentBiome();
        switch (currentBiome.Name)
        {
            case "Home":
                return grassStepSounds[index];
            case "Forest":
                return grassStepSounds[index];
            case "Desert":
                return sandStepSounds[index];
            case "Metal":
                return stoneStepSounds[index];
            case "Crystal":
                return stoneStepSounds[index];
            case "DarkMetal":
                return stoneStepSounds[index];
            default:
                return grassStepSounds[index];
        }
    }
}
