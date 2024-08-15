import {useEffect, useState} from 'react';
import { Input, Space, Button, DatePicker } from 'antd';
import { FolderFilled, FolderAddFilled, FileAddFilled, FileFilled, CloseOutlined, DeleteFilled } from '@ant-design/icons';
import dayjs from 'dayjs';
const { Search } = Input;
const { TextArea } = Input;

function MainPageComponent() {
    const [availableNodes, setAvailableNodes] = useState([]);
    const [hierarchy, setHierarchy] = useState('/');
    const [isDirectoryCreateFormVisible, setIsDirectoryCreateFormVisible] = useState(false);
    const [isDocumentCreateFormVisible, setIsDocumentCreateFormVisible] = useState(false);
    const [isShowInfoChosenDirectory, setIsShowInfoChosenDirectory] = useState(false);
    const [isShowInfoChosenDocument, setIsShowInfoChosenDocument] = useState(false);
    const [chosenNode, setChosenNode] = useState({})

    const onSearch = (value, _e, info) => console.log(info?.source, value);
    const token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiUHJvc3RvIiwiSWQiOiIyIiwiZXhwIjoxNzIzNzEwNjEwLCJpc3MiOiJNeUF1dGhTZXJ2ZXIiLCJhdWQiOiJNeUF1dGhDbGllbnQifQ.QFYli_ldShOtf_zbdY3h9vq_zpEPTksiWQNR-F77njs"
    async function createDirectory() {
        const name = document.querySelector('#inputDirectoryName').value;
        const response = await fetch('https://localhost:7018/api/documents', {
            method: 'POST', 
            headers: new Headers({ "Content-Type": "application/json", "Authorization": "Bearer " + token }), 
            body: JSON.stringify({
                type: "Directory",
                name: name,
                folderId: 0
            })
        });
        if (response.status == 201)
        {
            const json = await response.json();
            const createdAt = dayjs(json.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY')
            setAvailableNodes([...availableNodes, {...json, createdAt: createdAt}]);
            setIsDirectoryCreateFormVisible(false);
        }
    }

    async function createDocument() {
        const name = document.querySelector('#inputDocumentName').value;
        let outputDate = document.querySelector('#inputDocumentActivityDate').value;
        let activityDate = dayjs(outputDate, 'DD-MM-YYYY').format('YYYY-MM-DD');
        const content = document.querySelector('#inputDocumentContent').value;
        const date = dayjs(activityDate).toJSON();
        const response = await fetch('https://localhost:7018/api/documents', {
            method: 'POST', 
            headers: new Headers({"Content-Type": "application/json"}), 
            body: JSON.stringify({
                type: "Document",
                name: name,
                content: content,
                activityEnd: date,
                folderId: 0
            })
        });
        if (response.status == 201)
        {
            const json = await response.json();
            const activityEnd = dayjs(json.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY')
            const createdAt = dayjs(json.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY')
            setAvailableNodes([...availableNodes, {...json, activityEnd: activityEnd, createdAt: createdAt}]);
            setIsDocumentCreateFormVisible(false);
        }
    }

    function onChangeDateActivity(date, dateString) {
        // console.log(date, dateString);
    }

    async function deleteChosenNode() {
        const response = await fetch(`https://localhost:7018/api/documents/${chosenNode.id}`, {
            method: 'DELETE',
            headers: new Headers({"Content-Type": "application/json"}),
        });
        if (response.status == 200)
        {
            setAvailableNodes(availableNodes.filter((node) => node.id != chosenNode.id));
            setChosenNode({});
            setIsShowInfoChosenDirectory(false);
            setIsShowInfoChosenDocument(false);
        }
    }

    async function viewDirectoryContent(directory) {
        const response = await fetch(`api/NodeHierarchy/${directory.id}`);
    }

    async function getAvailableNodes() {
        const response = await fetch("https://localhost:7018/api/documents/all");
        if (response.status == 200)
        {
            const json = await response.json();
            json.map((node) => {
                if (node.type == "Document")
                {
                    const activityEnd = dayjs(node.activityEnd, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, activityEnd: activityEnd, createdAt: createdAt}]);
                }
                else {
                    const createdAt = dayjs(node.createdAt, 'YYYY-MM-DD').format('DD-MM-YYYY');
                    setAvailableNodes(prevNodes => [...prevNodes, {...node, createdAt: createdAt}]);
                }
            });
        }
    }

    useEffect(() => {
        getAvailableNodes();
    }, [])

    return (
        <>
            {isDirectoryCreateFormVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between'}}>
                            <Input placeholder='Введите название папки' id='inputDirectoryName' style={{width: '70%'}}/>
                            <CloseOutlined onClick={() => setIsDirectoryCreateFormVisible(false)}/>
                        </div>
                        <Button size='small' onClick={createDirectory} style={{width: "100px"}}>Сохранить</Button>
                    </div>
                </div>
            )}
            {isDocumentCreateFormVisible && (
                <div className='modal'>
                    <div className='modalContent'>
                        <div style={{display: 'flex', justifyContent: 'space-between'}}>
                            <Input placeholder='Введите название документа' id='inputDocumentName' style={{width: '70%'}}/>
                            <CloseOutlined onClick={() => setIsDocumentCreateFormVisible(false)}/>
                        </div>
                        <div style={{display: 'flex', gap: '10px', alignItems: 'center'}}>
                            <p>Дата окончания действия документа</p>
                            <DatePicker id="inputDocumentActivityDate" onChange={onChangeDateActivity} format={{format: 'DD-MM-YYYY'}} placeholder='Выберите дату' placement='bottomLeft'/>
                        </div>
                        <TextArea
                            placeholder="Введите содержимое документа"
                            autoSize={{
                                minRows: 6,
                                maxRows: 10
                            }}
                            id="inputDocumentContent"
                        />
                        <Button size='small' onClick={createDocument} style={{width: "100px"}}>Сохранить</Button>
                    </div>
                </div>
            )}
            <div>
                <Search
                    placeholder="Введите название документа или папки"
                    allowClear
                    onSearch={onSearch}
                    style={{
                        maxWidth: '700px',
                        width: '70%'
                    }}
                />
            </div>
            <br />
            <div style={{display: 'flex', justifyContent: 'space-between', height: '60px'}}>
                <div style={{display: 'flex', gap: '10px'}}>
                    <button onClick={() => setIsDirectoryCreateFormVisible(true)} className="btnWithIcon">
                        <FolderAddFilled style={{fontSize: '30px'}}/>
                        <p style={{textWrap: 'wrap', fontSize: '14px'}}>Создать папку</p>
                    </button>
                    <button onClick={() => setIsDocumentCreateFormVisible(true)} className="btnWithIcon">
                        <FileAddFilled style={{fontSize: '30px'}}/>
                        <p style={{textWrap: 'wrap', fontSize: '14px'}}>Создать документ</p>
                    </button>
                    <button onClick={() => deleteChosenNode()} className="btnWithIcon">
                        <DeleteFilled style={{fontSize: '30px'}}/>
                        <p style={{fontSize: '14px'}}>Удалить</p>
                    </button>
                </div>
                {isShowInfoChosenDirectory && !isShowInfoChosenDocument && <div style={{fontSize: '14px', maxSize: '40%'}}>
                    <p>Имя папки: {chosenNode.name}</p>
                    <p>Дата создания: {chosenNode.createdAt}</p>
                </div>}
                {isShowInfoChosenDocument && !isShowInfoChosenDirectory && <div style={{fontSize: '14px', maxSize: '40%'}}>
                    <p>Имя документа: {chosenNode.name}</p>
                    <p>Дата создания: {chosenNode.createdAt}</p>
                    <p>Дата активности: {chosenNode.activityEnd}</p>
                </div>}
            </div>
            <p style={{textAlign: 'left', margin: '10px 0'}}>{hierarchy}</p>
            <div>
                <Space wrap style={{gap: '15px'}}>
                    {availableNodes.map((node, index) => (
                        <div key={index} className='availableNode' style={{maxWidth: '120px'}}>
                            {node.type == "Directory" ? 
                            <button className="btnWithIcon" onClick={() => {setIsShowInfoChosenDocument(false); setChosenNode(node); setIsShowInfoChosenDirectory(true);}} onDoubleClick={() => viewDirectoryContent(node)}>
                                <FolderFilled style={{fontSize: '50px'}} />
                                <p style={{width: '110px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'}}>{node.name}</p>
                            </button>
                            :
                            <button className="btnWithIcon" onClick={() => {setIsShowInfoChosenDirectory(false); setChosenNode(node); setIsShowInfoChosenDocument(true);}}>
                                <FileFilled style={{fontSize: '45px'}}/>
                                <p style={{width: '110px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap'}}>{node.name}</p> 
                            </button>
                            }
                        </div>
                    ))}
                </Space>
            </div>
            
        </>
    );
}

export default MainPageComponent;