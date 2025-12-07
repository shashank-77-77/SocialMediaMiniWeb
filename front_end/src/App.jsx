import React from 'react'
import {Routes, Route} from 'react-router-dom'

const App = () => {
  return (
    <div>
    <Routes>
      <Route path="/" element={<Login/>}>
        <Route index element={<Feed/>}/>
        <Route path='messages' element={<Messages/>}/>
        <Route path='messages:/userId' element={<ChatBox/>}/>
        <Route path='connections' element={<Connections/>}/>
        <Route path='discover' element={<Discover/>}/>
        <Route path='profile' element={<Profile/>}/>
        <Route path='profile/:profileId' element={<Profile/>}/>
        <Route path='create-post' element={<CreatePost/>}/>
      </Route>
    </Routes>

    </div>
  )
}

export default App