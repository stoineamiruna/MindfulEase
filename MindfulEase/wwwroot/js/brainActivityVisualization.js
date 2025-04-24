const userId = '@User.Identity.GetUserId()'; 

fetch(`/api/emotion/user/${userId}/date/${selectedDate}/brain-activity`)
    .then(response => response.json())
    .then(data => {
        data.forEach(regionData => {
            const region = brain.getObjectByName(regionData.BrainRegion);
            if (region) {
                region.material.color.set(regionData.Color);
                region.material.opacity = regionData.Score; // Score este între 0-1
            }
        });
    });
