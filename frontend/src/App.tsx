function App() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-[#F5F5F5] font-sans">
      <div className="w-16 h-16 bg-[#0F172A] text-white rounded-2xl mb-6 shadow-lg flex items-center justify-center">
        <svg xmlns="http://www.w3.org/2000/svg" width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"><path d="M4 19.5v-15A2.5 2.5 0 0 1 6.5 2H20v20H6.5a2.5 2.5 0 0 1 0-5H20"/></svg>
      </div>
      <h1 className="text-4xl font-bold text-[#0F172A] mb-2 tracking-tight">MPOnline</h1>
      <p className="text-lg text-gray-500 mb-8 font-medium">Landing Page Under Development</p>
      
      <a href="/Account/Login" className="inline-flex items-center justify-center px-8 py-3 border border-transparent text-base font-medium rounded-xl text-white bg-[#2563EB] hover:bg-blue-700 shadow-md transition-all hover:-translate-y-0.5">
        Login
      </a>
    </div>
  );
}

export default App;
