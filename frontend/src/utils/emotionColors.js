const emotionMap = {
  happy: { color: '#66BB6A', icon: 'mdi-emoticon-happy-outline', label: 'Happy' },
  sad: { color: '#42A5F5', icon: 'mdi-emoticon-sad-outline', label: 'Sad' },
  stressed: { color: '#EF5350', icon: 'mdi-emoticon-angry-outline', label: 'Stressed' },
  anxious: { color: '#FFA726', icon: 'mdi-emoticon-confused-outline', label: 'Anxious' },
  calm: { color: '#26A69A', icon: 'mdi-meditation', label: 'Calm' },
  excited: { color: '#AB47BC', icon: 'mdi-emoticon-excited-outline', label: 'Excited' },
  grateful: { color: '#EC407A', icon: 'mdi-heart-outline', label: 'Grateful' },
  neutral: { color: '#78909C', icon: 'mdi-emoticon-neutral-outline', label: 'Neutral' },
}

export function getEmotionMeta(emotion) {
  const key = emotion?.toLowerCase()
  return emotionMap[key] || emotionMap.neutral
}

export function getEmotionColor(emotion) {
  return getEmotionMeta(emotion).color
}

export function getEmotionIcon(emotion) {
  return getEmotionMeta(emotion).icon
}

export function getAllEmotions() {
  return Object.keys(emotionMap)
}

export default emotionMap
