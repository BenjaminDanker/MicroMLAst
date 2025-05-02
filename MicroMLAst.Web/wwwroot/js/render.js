document.getElementById('btn').addEventListener('click', async () => {
  const code = document.getElementById('code').value;
  const container = document.getElementById('diagram');
  container.innerHTML = '<em>Rendering AST...</em>';

  try {
    const res = await fetch('/api/parse', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ code })
    });

    if (!res.ok) throw new Error('Server error');

    const { mermaid: graphDef } = await res.json();
    const graphId = 'graph-' + Date.now();
    const { svg, bindFunctions } = await mermaid.render(graphId, graphDef);

    container.innerHTML = svg;
    bindFunctions(container);
  } catch (err) {
    container.innerHTML = `<span style="color:#d32f2f;">Error: ${err.message}</span>`;
  }
});
