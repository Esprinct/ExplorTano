using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChartUI : MonoBehaviour
{
    [System.Serializable]
    public class PieSlice
    {
        public string id;
        public Image image;
    }

    [SerializeField] private List<PieSlice> slices = new List<PieSlice>();

    public void SetChart(List<PieChartEntry> entries)
    {
        
for (int i = 0; i < slices.Count; i++)
{
   
}
        float total = 0f;

        foreach (PieChartEntry entry in entries)
        {
            total += Mathf.Max(0f, entry.value);
        }

        float currentRotation = 0f;

        for (int i = 0; i < slices.Count; i++)
        {
            if (i >= entries.Count)
            {
                if (slices[i].image != null)
                {
                    slices[i].image.fillAmount = 0f;
                    slices[i].image.gameObject.SetActive(false);
                }

                continue;
            }

            Image image = slices[i].image;
            PieChartEntry entry = entries[i];

            if (image == null)
            {
                continue;
            }

            image.gameObject.SetActive(true);
       
            image.color = entry.color;

            float ratio = total > 0f ? Mathf.Max(0f, entry.value) / total : 0f;

            image.type = Image.Type.Filled;
            image.fillMethod = Image.FillMethod.Radial360;
            image.fillAmount = ratio;

            image.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -currentRotation * 360f);

            currentRotation += ratio;
        }
    }

    public void ClearChart()
    {
        foreach (PieSlice slice in slices)
        {
            if (slice.image != null)
            {
                slice.image.fillAmount = 0f;
                slice.image.gameObject.SetActive(false);
            }
        }
    }
}